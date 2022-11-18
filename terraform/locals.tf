locals {
  # General
  project       = "corebridge-367900"
  project_short = "corebridge"
  env           = "dev"
  sig           = "${local.project_short}-${local.env}"

  # Tags
  # Declare default labels. If you want to merge another labels, like this:
  #
  # ```
  #  labels = "${merge(local.default_labels,
  #               {
  #                 foo = "bar",
  #                 ...
  #               }
  #            )}"
  # ```
  #
  default_labels = {
    project    = "${local.project_short}"
    env        = "${local.env}"
    managed-by = "terraform"
  }
}
