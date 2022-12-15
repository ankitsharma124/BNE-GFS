locals {
  repository_id = "${google_artifact_registry_repository.corebridge.location}-docker.pkg.dev/${local.project}/${google_artifact_registry_repository.corebridge.repository_id}"
  image_tag     = "test3"
  image_uri     = "${local.repository_id}/corebridge:${local.image_tag}"

  # NOTE:
  #   Use this cred in application. This sholud be replace into secret.
  google_application_credentials = "/app/bne-gfs-06ffad36d462.json"
}


resource "google_cloud_run_service" "corebridge-seconda" {
  location                   = "asia-northeast1"
  name                       = "corebridge"
  autogenerate_revision_name = true

  template {
    metadata {
      annotations = {
        "autoscaling.knative.dev/maxScale"        = "100"
        "run.googleapis.com/client-name"          = "cloud-console"
        "run.googleapis.com/vpc-access-connector" = google_vpc_access_connector.cloudrun.id
        "run.googleapis.com/vpc-access-egress"    = "private-ranges-only"
      }
    }

    spec {
      container_concurrency = 80
      # Compute Engine default service account
      service_account_name = "116615950553-compute@developer.gserviceaccount.com"
      timeout_seconds      = 300

      containers {
        image = local.image_uri
        env {
          name  = "ConnectionStrings__Spanner"
          value = "Data Source=projects/bne-gfs/instances/corebridge-development/databases/corebridge"
        }
        env {
          name  = "ConnectionStrings__Redis"
          value = "${google_redis_instance.corebridge.host}:${google_redis_instance.corebridge.port}"
        }
        env {
          name  = "GOOGLE_APPLICATION_CREDENTIALS"
          value = local.google_application_credentials
        }
        ports {
          container_port = 80
          name           = "http1"
        }
        resources {
          limits = {
            "cpu"    = "1000m"
            "memory" = "512Mi"
          }
          requests = {}
        }
      }
    }
  }

  traffic {
    percent       = 0
    revision_name = "corebridge-7tjm5"
    tag           = "green"
  }

  traffic {
    percent       = 100
    revision_name = "corebridge-lr4vl"
    tag           = "blue"
  }

  # NOTE: Add hidden dependency
  depends_on = [
    google_spanner_database.corebridge,
  ]
}
