resource "google_project_service" "artifactregistry" {
  service = "artifactregistry.googleapis.com"
}

## refs: https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/artifact_registry_repository
resource "google_artifact_registry_repository" "corebridge" {
  repository_id = "corebridge"
  location      = "asia"
  description   = "corebridge docker repository"
  format        = "DOCKER"

  labels     = local.default_labels
  depends_on = [google_project_service.artifactregistry]
}


output "registry_corebridge_host" {
  description = "you may need to run 'gcloud auth configure-docker <this value>.'"
  value       = "${google_artifact_registry_repository.corebridge.location}-docker.pkg.dev"
}

output "registry_corebridge_image_name_prefix" {
  description = "image name prefix for corebridge registry"
  value       = "${google_artifact_registry_repository.corebridge.location}-docker.pkg.dev/${local.project}/${google_artifact_registry_repository.corebridge.repository_id}"
}

