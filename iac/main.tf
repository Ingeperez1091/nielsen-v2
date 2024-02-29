data "aws_region" "current" {}

data "aws_vpc" "data_vpc" {
  tags = {
    Name = "Main VPC - Services Data-${terraform.workspace} - ${data.aws_region.current.name}"
  }
}

data "aws_subnet" "data_dev_subnet_0" {
  id = var.subnet_id
}

data "archive_file" "lambda_archive" {
  type = "zip"

  source_dir  = "${var.working_directory}/publish"
  output_path = "vizio-nielsen-reporting-lambda.zip"
}
# Create IAM role for Lambda connected to VPC
resource "aws_iam_role" "lambda_role" {
  name = "nielsenreportstack-lambda-role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Sid    = ""
        Principal = {
          Service = "lambda.amazonaws.com"
        }
      },
    ]
  })
}

resource "aws_secretsmanager_secret" "nielsen_report_configurations" {
  name                    = "NielsenReport/Configuration"
  recovery_window_in_days = 0
}

resource "aws_secretsmanager_secret_version" "nielsen_report_configurations" {
  secret_id = aws_secretsmanager_secret.nielsen_report_configurations.id

  secret_string = jsonencode({
    UniversalDbConnectionString = "replace_me"
    NotificationSettings        = "replace_me"
    ChannelReportSettings       = "replace_me"
  })

  lifecycle {
    ignore_changes = [
      # Let's not manage the
      secret_string
    ]
  }
}

resource "aws_iam_role_policy_attachment" "lambda_vpc_access_attachment" {
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
  role       = aws_iam_role.lambda_role.name
}

resource "aws_iam_policy" "nielsen_report_custom_policy" {
  name   = "nielsen_report_custom_policy"
  policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "VisualEditor0",
            "Effect": "Allow",
            "Action": [
                "ses:SendEmail",
                "ses:SendTemplatedEmail",
                "secretsmanager:GetSecretValue",
                "secretsmanager:DescribeSecret",
                "ses:SendRawEmail",
                "secretsmanager:ListSecrets"
            ],
            "Resource": "*"
        }
    ]
}
EOF
}

resource "aws_iam_role_policy_attachment" "custom_policy_attachment" {
  policy_arn = aws_iam_policy.nielsen_report_custom_policy.arn
  role       = aws_iam_role.lambda_role.name
}

resource "aws_security_group" "lambda_security_group" {
  name        = "nielsenreportstack-lambda-sg"
  description = "Security group for lambda function"
  vpc_id      = data.aws_vpc.data_vpc.id
  egress {
    description = "Allow all egress traffic"
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_lambda_function" "nielsen_report_lambda_function" {
  filename         = data.archive_file.lambda_archive.output_path
  function_name    = "nielsenreportstack"
  role             = aws_iam_role.lambda_role.arn
  handler          = "NielsenChannelsReporting::NielsenChannelsReporting.Functions_GenerateReportAsync_Generated::GenerateReportAsync"
  source_code_hash = data.archive_file.lambda_archive.output_base64sha256
  runtime          = "dotnet6"
  timeout          = 300
  memory_size      = 512
  publish          = true
  vpc_config {
    subnet_ids         = [data.aws_subnet.data_dev_subnet_0.id]
    security_group_ids = [aws_security_group.lambda_security_group.id]
  }
}

resource "aws_cloudwatch_event_rule" "nielsen_report_schedule" {
  name        = "nielsen_report_schedule"
  description = "Schedule for Nielsen Report"
  # Schedule lambda to send out once a week on Monday at 8:00 AM UTC
  schedule_expression = "cron(0 8 ? * MON *)"
}

resource "aws_cloudwatch_event_target" "nielsen_report_schedule_target" {
  rule      = aws_cloudwatch_event_rule.nielsen_report_schedule.name
  target_id = "nielsen_report_schedule_target"
  arn       = aws_lambda_function.nielsen_report_lambda_function.arn
}

resource "aws_lambda_permission" "allow_cloudwatch_to_call_check_foo" {
  statement_id  = "AllowExecutionFromCloudWatch"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.nielsen_report_lambda_function.function_name
  principal     = "events.amazonaws.com"
  source_arn    = aws_cloudwatch_event_rule.nielsen_report_schedule.arn
}
