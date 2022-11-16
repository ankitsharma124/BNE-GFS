# NOTE:
#  Use standard tier for production.
resource "google_redis_instance" "corebridge" {
  name               = "corebridge"
  display_name       = "corebridge"
  memory_size_gb     = 2
  redis_version      = "REDIS_6_X"
  tier               = "BASIC"
  authorized_network = "default"

  persistence_config {
    persistence_mode = "DISABLED"
  }
}
