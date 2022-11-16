resource "google_project_service" "artifactregistry" {
  service = "artifactregistry.googleapis.com"
}

## refs: https://registry.terraform.io/providers/hashicorp/google/latest/docs/resources/artifact_registry_repository
resource "google_artifact_registry_repository" "corebridge" {
  repository_id = "corebridge"
  location      = "asia"
  description   = "corebridge docker repository"
  format        = "DOCKER"
  depends_on = [google_project_service.artifactregistry]
}


resource "google_service_account" "registry_admin" {
  account_id   = "registry-admin"
  display_name = "Service Account to manage images on registry"
}

resource "google_artifact_registry_repository_iam_member" "registry_admin" {
  location   = google_artifact_registry_repository.corebridge.location
  repository = google_artifact_registry_repository.corebridge.name
  role       = "roles/artifactregistry.repoAdmin"
  member     = "serviceAccount:${google_service_account.registry_admin.email}"
}

output "registry_service_account_email" {
  description = "service account for registry"
  value       = google_service_account.registry_admin.email
}

output "registry_corebridge_host" {
  description = "you may need to run 'gcloud auth configure-docker <this value>.'"
  value       = "${google_artifact_registry_repository.corebridge.location}-docker.pkg.dev"
}

output "registry_corebridge_image_name_prefix" {
  description = "image name prefix for corebridge registry"
  value       = "${google_artifact_registry_repository.corebridge.location}-docker.pkg.dev/corebridge-367900/${google_artifact_registry_repository.corebridge.repository_id}"
}

