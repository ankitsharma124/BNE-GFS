resource "google_spanner_instance" "corebridge-development" {
  display_name  = "CoreBridge-development"
  name          = "corebridge-development"
  force_destroy = false
  config        = "regional-asia-southeast2"
  num_nodes     = 0
  labels        = local.default_labels
}

resource "google_spanner_database" "corebridge" {
  deletion_protection      = true
  instance                 = google_spanner_instance.corebridge-development.name
  name                     = "corebridge"
  version_retention_period = "1h"
}
