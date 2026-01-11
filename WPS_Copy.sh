#----------------------Path Selection-------------------------->
fromPath=/root/Build_WRF
toPath=/root/Build_WRF/WRF_Run/Run_WRF

ulimit -s unlimited

ln -sf $fromPath/WPS-4.1/geogrid.exe $toPath
ln -sf $fromPath/WPS-4.1/ungrib.exe $toPath
ln -sf $fromPath/WPS-4.1/metgrid.exe $toPath

cp -rf $fromPath/WPS-4.1/geogrid $toPath
cp -rf $fromPath/WPS-4.1/metgrid $toPath
cp /root/Build_WRF/WRF_Run/Nested_Namelist/namelist.wps $toPath

cp $fromPath/WPS-4.1/link_grib.csh $toPath
cp $fromPath/WPS-4.1/ungrib/Variable_Tables/Vtable.GFS $toPath
cp $fromPath/WPS-4.1/ungrib/Variable_Tables/Vtable.SST $toPath




