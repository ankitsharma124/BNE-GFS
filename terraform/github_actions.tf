locals {
  github_repo = "scopenext/CoreBridge"
}

module "gh_oidc" {
  source      = "terraform-google-modules/github-actions-runners/google//modules/gh-oidc"
  project_id  = "corebridge-367900"
  pool_id     = "github-pool"
  provider_id = "github-pool-provider"
  sa_mapping = {
    (google_service_account.registry_admin.id) = {
      sa_name   = google_service_account.registry_admin.name
      attribute = "attribute.repository/${local.github_repo}"
    }
  }
}

output "github_actions_workload_identity_pool_provider" {
  description = "Workload Identity Pool Provider ID"
  value       = module.gh_oidc.provider_name
}

