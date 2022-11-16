resource "google_storage_bucket" "corebridge-app" {
  name          = "corebridge-app"
  force_destroy = false
  location      = "ASIA"
  storage_class = "STANDARD"
  versioning {
    enabled = false
  }
}
