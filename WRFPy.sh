#!/bin/bash
ulimit -s unlimited
export LD_LIBRARY_PATH="$LD_LIBRARY_PATH:/root/WRF/LIBRARIES/grib2/lib"
cd /root/WRF/RUN_WRF
/root/anaconda3/bin/python WRF4.1.2_Final_updating.py

