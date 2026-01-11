#----------------------Path Selection-------------------------->
fromPath=/root/WRF
toPath=/root/WRF/RUN_WRF

ulimit -s unlimited

ln -sf $fromPath/WPS/geogrid.exe $toPath
ln -sf $fromPath/WPS/ungrib.exe $toPath
ln -sf $fromPath/WPS/metgrid.exe $toPath

# copy the two folders geogrid and metgrid. Those contain
# tables telling geogrid and metgrid how we should interpolate
# usually you would copy metgrid from the same directory as geogrid
cp -rf $fromPath/WPS/geogrid $toPath
cp -rf $fromPath/WPS/metgrid $toPath
#cp /root/Build_WRF/WRF_Run/Nested_Namelist/namelist.wps $toPath


# In addition we need two files to get ungrib up and running
# the Vtable contains information on the variable codes in the 
# met input data
cp $fromPath/WPS/link_grib.csh $toPath
cp $fromPath/WPS/ungrib/Variable_Tables/Vtable.GFS $toPath
cp $fromPath/WPS/ungrib/Variable_Tables/Vtable.SST $toPath
#cp -rf /home/user/data/fcast2011101318/Vtable* .
#cp -rf /home/user/data/fcast2011101318/run_ungrib.sh . 
#cp -rf /home/user/data/fcast2011101318/foolSST.sh .
#mv metgrid/metgrid.tbl metgrid/METGRID.TBL





# get the namelist.input, and edit accordingly
#cp /root/Build_WRF/WRF_Run/Nested_Namelist/namelist.input $toPath
#cp $fromPath/WPS/foolSST.sh $toPath

#link met_em
#not necessary to link due to same folder
# we also need some tables telling wrf what the
# different landuse, green house gasses, and ozone specs
# to use
cp -rf $fromPath/WRFV3/run/*TBL $toPath
cp -rf $fromPath/WRFV3/run/*DATA* $toPath
cp -rf $fromPath/WRFV3/run/ozone* $toPath
cp -rf $fromPath/WRFV3/run/co2* $toPath

# link in the executables 
ln -sf $fromPath/WRFV3/run/real.exe $toPath
ln -sf $fromPath/WRFV3/run/wrf.exe $toPath

#Change namelist.input



# post processing
#cp -rf $fromPath/ARWpost/src $toPath
#cp -rf $fromPath/ARWpost/arch $toPath
#cp $fromPath/ARWpost/namelist.ARWpost $toPath
#ln -sf $fromPath/ARWpost/ARWpost.exe $toPath



