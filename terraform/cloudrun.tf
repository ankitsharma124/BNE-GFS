resource "google_cloud_run_service" "corebridge-seconda" {
  location = "asia-northeast1"
  name     = "corebridge"

  template {
    metadata {
      annotations = {
        "autoscaling.knative.dev/maxScale"        = "100"
        "run.googleapis.com/client-name"          = "cloud-console"
        "run.googleapis.com/vpc-access-connector" = "projects/corebridge-367900/locations/asia-northeast1/connectors/cloudrun-vpc"
        "run.googleapis.com/vpc-access-egress"    = "private-ranges-only"
      }
    }

    spec {
      container_concurrency = 80
      service_account_name  = "978233375489-compute@developer.gserviceaccount.com"
      timeout_seconds       = 300

      containers {
        image   = "asia-docker.pkg.dev/corebridge-367900/corebridge/corebridge:latest"
        env {
          name  = "ASPNETCORE_ENVIRONMENT"
          value = "DockerLocal"
        }
        env {
          name  = "ConnectionStrings__Redis"
          value = "10.91.188.179:6379"
        }
        env {
          name  = "GOOGLE_APPLICATION_CREDENTIALS"
          value = "/app/corebridge-367900-75f6bcea9581.json"
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
    latest_revision = true
    percent         = 100
  }
}

