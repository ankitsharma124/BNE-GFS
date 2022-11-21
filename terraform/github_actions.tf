locals {
  github_repo = "scopenext/CoreBridge"
}

#module "gh_oidc" {
#  source      = "terraform-google-modules/github-actions-runners/google//modules/gh-oidc"
#  project_id  = local.project
#  pool_id     = "github-pool"
#  provider_id = "github-pool-provider"
#  sa_mapping = {
#    (google_service_account.registry_admin.id) = {
#      sa_name   = google_service_account.registry_admin.name
#      attribute = "attribute.repository/${local.github_repo}"
#    }
#  }
#}
#
#output "github_actions_workload_identity_pool_provider" {
#  description = "Workload Identity Pool Provider ID"
#  value       = module.gh_oidc.provider_name
#}


resource "google_service_account" "terraform" {
  account_id = "terraform-sa"
}

resource "google_project_iam_member" "terraform" {
  role    = "roles/editor"
  project = local.project
  member  = "serviceAccount:${google_service_account.terraform.email}"
}

module "gh_oidc" {
  source      = "terraform-google-modules/github-actions-runners/google//modules/gh-oidc"
  project_id  = local.project
  pool_id     = "github-tf-pool"
  provider_id = "github-tf-pool-provider"
  sa_mapping = {
    (google_service_account.terraform.id) = {
      sa_name   = google_service_account.terraform.name
      attribute = "attribute.repository/${local.github_repo}"
    }
  }
}


output "github_actions_workload_identity_pool_provider" {
  description = "Workload Identity Pool Provider ID"
  value       = module.gh_oidc.provider_name
}

output "registry_service_account_email" {
  description = "service account for terraform"
  value       = google_service_account.terraform.email
}

