# Vizio Nielsen Reporting
This repository contains a solution for generate automated reports to Nielsen.
    
  <!-- Start Document Outline -->

* [Overview](#overview)
* [IaC](#iac)
	* [backend](#backend)
	* [main](#main)
	* [providers](#providers)
	* [variables](#variables)
* [Workflows:](#workflows)
	* [CI-pipeline](#ci-pipeline)
	* [Terraform-Plan-On-PR-To-branch_name](#terraform-plan-on-pr-to-branch_name)
	* [Terraform-Apply-On-Merge-Into-branchname](#terraform-apply-on-merge-into-branchname)
* <a href="NielsenChannelsReporting/NielsenChannelsReporting/Readme.md" target="_blank">code</a>

<!-- End Document Outline -->
  

## Overview

The purpose of this solution is to automatically send reports to Nielsen with the list and some details of added channels. 

The architecture and sequence diagrams are shown below:
  
<figure>
    <img src="DocAssets/Architecture%20Diagram.png" alt="Architecture Diagram" style="background-color:white;">
    <figcaption>Architecture Diagram</figcaption>
</figure>  


<figure>
    <img src="DocAssets/Sequence%20Diagram.png" alt="Sequence Diagram" style="background-color:white;">
    <figcaption>Sequence Diagram</figcaption>
</figure>  

<div style='page-break-after: always'></div>

## IaC

This folder contains the terraform files *(.tf)* required to specify and generate the cloud infrastructure required to deploy the solution: 

### backend 
This file specifies the S3 bucket and dynamodb_table where the zip file that contains the lambda package will be stored.
  
### main
This file defines the cloud services, resources and their connections required to implement the solution:

*  *nielsenreportstack-lambda-role:* IAM role for Lambda connected to VPC.
*  *nielsen_report_configurations:* AWS secret to store the credentials and sensitive information.
*  *nielsen_report_custom_policy:* AWS IAM policy that defines the permissions given to the lambda function to access to the services it will use, ie: SES, Secret Manager, etc.
*  *nielsenreportstack-lambda-sg:* Security group for lambda function.
*  *nielsenreportstack:* Lambda function.
*  *nielsen_report_schedule:* Cloudwatch event rule that defines the schedule for sending the Nielsen Report.
*  *allow_cloudwatch_to_call_check_foo*: Permission given to Cloudwatch to trigger the lambda function.

### providers
This file specifies the required providers with the minimun version for terraform.

### variables
This file defines the variables that will be used in the release and that will vary on each environment:  
  
* *subnet_id:* The ID of the subnet to create the lambda in
* *working_directory:* The directory to run the terraform commands from
 

For each environment *(dev,stage and prod)* there is a folder with files that overrides the *backend.tf* and *variables.tf* files with the respective value for the environment.

## Workflows:  
  
The CI/CD pipelines are defined in the **.github/workflows** folder:

### CI-pipeline
This file defines the steps to follow when a PR is raised on develop or master branches:  

1. Setup NetCore
2. Restore dependencies
3. Build solution
4. Run unit tests

The *workflow_dispatch* tag allows to run manually this pipeline from Github without need of create a pull request or refresh the current PR making a push to the remote branch.
  

### Terraform-Plan-On-PR-To-*branch_name*
This file allows to display the changes that will be applied on the cloud environment(s) according to the destination branch. The defined jobs are:
  
#### changes:

1. run workflows_config/grab_json_elements.py file to get configurations and details for the selected environment(s) based on the destination bracnch. The results are stored in the *folders* output.

  
#### terraform: 

This job runs steps required to validate the infraestructure and get the changes planned to apply. the steps are:
  
1. Build and publish the package
2. Setup terraform
3. Configure AWS credentials
4. Init terraform
5. Get terraform
6. Set workspace
7. Format
8. Validate
9. Plan
10. Run script to print the plan.


### Terraform-Apply-On-Merge-Into-*branchname*
This file allows to make the infraestructure updates and deploy the lambda on the cloud environment(s) according to the destination branch.


1. Build and publish the package
2. Setup terraform
3. Configure AWS credentials
4. Apply changes.

The  **.github/workflows_configs** folder contains the following files:

* *grab_json_elements*: a python file that will read data from **tf-configs.json** and will return the conviguration values for the selected environment(s) passed in the input.
* *tf-configs*: a JSON file that stores the configurations for each environment.


The configuration for connect Gihub with AWS is described in <a href="https://vizio.atlassian.net/wiki/spaces/AD/pages/235510825012/Securing+AWS+deployments+from+GitHub+Actions+with+OIDC" target="_blank">Securing AWS Deployments From GitHub Actions With OIDC</a>.

## Code
The code is in NielsenChannelsReporting folder, the documentation is <a href="NielsenChannelsReporting/NielsenChannelsReporting/Readme.md" target="_blank">here</a>.