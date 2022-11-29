resource "random_id" "corebridge-bucket" {
  byte_length = 8
}

resource "google_storage_bucket" "corebridge-app" {
  name          = "corebridge-app-${random_id.corebridge-bucket.hex}"
  force_destroy = false
  location      = "ASIA"
  storage_class = "STANDARD"
  versioning {
    enabled = false
  }

  labels = local.default_labels
}
