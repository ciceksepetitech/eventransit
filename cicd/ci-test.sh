#!/bin/bash
set -e

# variables
configuration=${1:-Debug}

/bin/bash ./.build/src/test.sh $configuration
