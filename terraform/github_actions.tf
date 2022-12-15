locals {
  github_repo = "scopenext/BNE-GFS"
}


resource "google_service_account" "github" {
  account_id   = "github-sa"
  display_name = "A service account for GitHub Actions"
  description  = "link to Workload Identity Pool used by github actions"
}

#resource "google_project_iam_member" "github" {
#  project = local.project
#  role    = "roles/editor"
#  member  = "serviceAccount:${google_service_account.github.email}"
#}


#module "gh_oidc" {
#  source      = "terraform-google-modules/github-actions-runners/google//modules/gh-oidc"
#  project_id  = local.project
#  pool_id     = "github-tf-pool"
#  provider_id = "github-tf-pool-provider"
#  sa_mapping = {
#    (google_service_account.github.id) = {
#      sa_name   = google_service_account.github.name
#      attribute = "attribute.repository/${local.github_repo}"
#    }
#  }
#}


# NOTE: 
#   This value comes to GitHub's `secrets.GOOGLE_IAM_WORKLOAD_IDENTITY_POOL_PROVIDER`
#
#output "github_actions_workload_identity_pool_provider" {
#  description = "Workload Identity Pool Provider ID"
#  value       = module.gh_oidc.provider_name
#}
#
## NOTE: 
##   This value comes to GitHub's `secrets.SERVICE_ACCOUNT_EMAIL`
##
#output "registry_service_account_email" {
#  description = "service account for terraform"
#  value       = google_service_account.terraform.email
#}
#
