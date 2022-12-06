# Wait for Permission added
resource "google_vpc_access_connector" "cloudrun" {
  name           = "cloudrun-vpc"
  network        = "default"
  ip_cidr_range  = "10.0.0.0/28"
  machine_type   = "e2-micro"
  max_instances  = 10
  max_throughput = 1000
  min_instances  = 2
  min_throughput = 200
}
