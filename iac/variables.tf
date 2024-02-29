variable "subnet_id" {
  description = "The ID of the subnet to create the lambda in"
  default     = "subnet-0deb463cf3ad4c0b2" # dev subnet
}

variable "working_directory" {
  description = "The directory to run the terraform commands from"
  default     = "../NielsenChannelsReporting/NielsenChannelsReporting"
}