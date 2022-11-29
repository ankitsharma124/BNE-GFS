#locals {
#  repository_id = "${google_artifact_registry_repository.corebridge.location}-docker.pkg.dev/${local.project}/${google_artifact_registry_repository.corebridge.repository_id}"
#  image_tag     = "latest"
#  image_uri     = "${local.repository_id}/corebridge:${local.image_tag}"
#
#  # NOTE:
#  #   Use this cred in application. This sholud be replace into secret.
#  google_application_credentials = "/app/corebridge-367900-75f6bcea9581.json"
#}
#
#
#resource "google_cloud_run_service" "corebridge-seconda" {
#  location = "asia-northeast1"
#  name     = "corebridge"
#
#  template {
#    metadata {
#      annotations = {
#        "autoscaling.knative.dev/maxScale"        = "100"
#        "run.googleapis.com/client-name"          = "cloud-console"
#        "run.googleapis.com/vpc-access-connector" = google_vpc_access_connector.cloudrun.id
#        "run.googleapis.com/vpc-access-egress"    = "private-ranges-only"
#      }
#    }
#
#    spec {
#      container_concurrency = 80
#      service_account_name  = "978233375489-compute@developer.gserviceaccount.com"
#      timeout_seconds       = 300
#
#      containers {
#        image = local.image_uri
#        env {
#          name  = "ConnectionStrings__Redis"
#          value = "${google_redis_instance.corebridge.host}:${google_redis_instance.corebridge.port}"
#        }
#        env {
#          name  = "GOOGLE_APPLICATION_CREDENTIALS"
#          value = local.google_application_credentials
#        }
#        ports {
#          container_port = 80
#          name           = "http1"
#        }
#        resources {
#          limits = {
#            "cpu"    = "1000m"
#            "memory" = "512Mi"
#          }
#          requests = {}
#        }
#      }
#    }
#  }
#
#  traffic {
#    latest_revision = true
#    percent         = 100
#  }
#
#  # NOTE: Add hidden dependency
#  depends_on = [
#    google_spanner_database.corebridge,
#  ]
#}

