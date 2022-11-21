provider "google" {
  project = "corebridge-367900" # TODO: Use as variable
  region  = "asia-northeast1"
  zone    = "asia-northeast1-a"
}

terraform {
  backend "gcs" {
    bucket = "tf-state-934af1408b5cdc78" # google_storage_bucket.tfstate.name
    prefix = "terraform/state"
  }
}

resource "random_id" "bucket_suffix" {
  byte_length = 8
}

resource "google_storage_bucket" "tfstate" {
  name          = "tf-state-${random_id.bucket_suffix.hex}"
  force_destroy = false
  location      = "asia-northeast1"
  storage_class = "STANDARD"
  versioning {
    enabled = true
  }
  labels = local.default_labels
}
