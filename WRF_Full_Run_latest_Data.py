import os
from datetime import datetime
import urllib.request
from datetime import timedelta
import subprocess
from shutil import copyfile

forecastDateTemp = datetime.now()
#ForecastDay = forecastDateTemp.day
ForecastDay = 19
ForecastMonth = forecastDateTemp.month
ForecastYear = forecastDateTemp.year
forecastTimeTemp = 12 # forecast hours
forecastDate= datetime(ForecastYear, ForecastMonth, ForecastDay, forecastTimeTemp )
timehour = forecastDate.strftime('%H')

print (timehour)

def exe_Run(cmd): # this function run all shell script and exe file
    p = subprocess.Popen(cmd, shell=True, stdout=subprocess.PIPE)
    for line in p.stdout:
        print (line)
    p.wait()
    print (p.returncode)

#print(forecastDate.strftime('%Y%m%d'))
path = r"/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF" # model location
pathData = r"/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/Data" # Data location
timeDelta = 4 # Forecast duration in day
dataRange = timeDelta * 24 + 1

if not os.path.exists(pathData+'//'+ forecastDate.strftime('%Y-%m-%d_%H')):
    os.makedirs(pathData+'//'+ forecastDate.strftime('%Y-%m-%d_%H'))

for x in range(0, dataRange, 3):
    if(x<10):
        urllib.request.urlretrieve("https://nomads.ncep.noaa.gov/cgi-bin/filter_gfs_0p50.pl?file=gfs.t"+ timehour +"z.pgrb2full.0p50.f00"+str(x)+"&all_lev=on&all_var=on&subregion=&leftlon=59&rightlon=114&toplat=52&bottomlat=-2&dir=%2Fgfs."+forecastDate.strftime('%Y%m%d')+"%2F"+ timehour, pathData +'//'+ forecastDate.strftime('%Y-%m-%d_%H')+'//'+'gfs.t'+ timehour +'z.pgrb2.0p50.f00'+str(x))
    elif(x<100):
        urllib.request.urlretrieve("https://nomads.ncep.noaa.gov/cgi-bin/filter_gfs_0p50.pl?file=gfs.t"+ timehour +"z.pgrb2full.0p50.f0"+str(x)+"&all_lev=on&all_var=on&subregion=&leftlon=59&rightlon=114&toplat=52&bottomlat=-2&dir=%2Fgfs."+forecastDate.strftime('%Y%m%d')+"%2F"+ timehour, pathData +'//'+ forecastDate.strftime('%Y-%m-%d_%H')+'//'+'gfs.t'+ timehour +'z.pgrb2.0p50.f0'+str(x))
    else:
        urllib.request.urlretrieve("https://nomads.ncep.noaa.gov/cgi-bin/filter_gfs_0p50.pl?file=gfs.t"+ timehour +"z.pgrb2full.0p50.f"+str(x)+"&all_lev=on&all_var=on&subregion=&leftlon=59&rightlon=114&toplat=52&bottomlat=-2&dir=%2Fgfs."+forecastDate.strftime('%Y%m%d')+"%2F"+ timehour, pathData +'//'+ forecastDate.strftime('%Y-%m-%d_%H')+'//'+'gfs.t'+ timehour +'z.pgrb2.0p50.f'+str(x))
print('GFS file download is completed')

urllib.request.urlretrieve("ftp://ftpprd.ncep.noaa.gov/pub/data/nccf/com/gfs/prod/sst."+ (forecastDate-timedelta(days=1)).strftime('%Y%m%d') +"/rtgssthr_grb_0.083.grib2",pathData+'//'+ forecastDate.strftime('%Y-%m-%d_%H')+'//'+'rtgssthr_grb_0.083.grib2')
print('SST data download is completed')

#-----------geogrid.exe run ---------------------- need only one-------
#cmdgeogrid = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/geogrid.exe', '--arg', 'value']
#exe_Run(cmdgeogrid)
#---------------------End geogrid.exe run-------------------------

gfsFileDelete = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/rm -rf SST*', '--arg', 'value']
sstFileDelete = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/rm -rf GFS*', '--arg', 'value']
metFileDelete = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/rm -rf met_em.d01.*', '--arg', 'value']
exe_Run(gfsFileDelete)
exe_Run(sstFileDelete)
exe_Run(metFileDelete)
start_date = " start_date = '" + forecastDate.strftime('%Y-%m-%d_%H') +":00:00','"+ forecastDate.strftime('%Y-%m-%d_%H') +":00:00',\n"
end_date = " end_date   = '" + (forecastDate+timedelta(timeDelta)).strftime('%Y-%m-%d_%H') +":00:00','"+ (forecastDate+timedelta(timeDelta)).strftime('%Y-%m-%d_%H') +":00:00',\n"


nameListWPS = open(path+'//namelist.wps', 'r+').readlines()
nameListWPS[3]=start_date
nameListWPS[4] = end_date
nameListWPS[45]=" prefix = 'GFS',\n"
textTempWPS = open(path+'//namelist.wps', 'w+')
textTempWPS.writelines(nameListWPS)
textTempWPS.close()

cmdGFSLink = ['ln -sf '+ pathData+ '/'+ forecastDate.strftime('%Y-%m-%d_%H')+"/gfs* .", '--arg', 'value']
exe_Run(cmdGFSLink)
cmdGFSAAALink = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/link_grib.csh gfs* .', '--arg', 'value']
exe_Run(cmdGFSAAALink)
os.rename("Vtable.GFS", "Vtable")
cmdUngribGfaRun = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/ungrib.exe', '--arg', 'value']
exe_Run(cmdUngribGfaRun)
os.rename("Vtable", "Vtable.GFS")
os.rename("Vtable.SST", "Vtable")
cmdSSTLink = ['ln -sf '+ pathData+ '/'+ forecastDate.strftime('%Y-%m-%d_%H')+"/rtgsst* .", '--arg', 'value']
exe_Run(cmdSSTLink)
cmdSSTAALink = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/link_grib.csh rtg* .', '--arg', 'value']
exe_Run(cmdSSTAALink)

start_date = " start_date = '" + (forecastDate-timedelta(days=1)).strftime('%Y-%m-%d') +":00:00','"+ (forecastDate-timedelta(days=1)).strftime('%Y-%m-%d') +":00:00',\n"
end_date = " end_date   = '" + (forecastDate-timedelta(days=1)).strftime('%Y-%m-%d') +":00:00','"+ (forecastDate-timedelta(days=1)).strftime('%Y-%m-%d') +":00:00',\n"

nameListWPS = open(path+'//namelist.wps', 'r+').readlines()
nameListWPS[3]=start_date
nameListWPS[4] = end_date
nameListWPS[45]=" prefix = 'SST',\n"
textTempWPS = open(path+'//namelist.wps', 'w+')
textTempWPS.writelines(nameListWPS)
textTempWPS.close()

cmdUngribSSTRun = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/ungrib.exe', '--arg', 'value']
exe_Run(cmdUngribSSTRun)

#import datetime
tempDate = datetime(forecastDate.year, forecastDate.month, forecastDate.day, 00, 00)
for x in range(0, dataRange,3): # sstFileCopy
    copyfile("SST:"+ (tempDate-timedelta(days=1)).strftime('%Y-%m-%d')+"_00", "SST:"+ (tempDate+timedelta(hours=x)).strftime('%Y-%m-%d_%H'))

os.rename("Vtable","Vtable.SST")

start_date = " start_date = '" + forecastDate.strftime('%Y-%m-%d_%H') +":00:00','"+ forecastDate.strftime('%Y-%m-%d_%H') +":00:00',\n"
end_date = " end_date   = '" + (forecastDate+timedelta(timeDelta)).strftime('%Y-%m-%d_%H') +":00:00','"+ (forecastDate+timedelta(timeDelta)).strftime('%Y-%m-%d_%H') +":00:00',\n"

nameListWPS = open(path+'//namelist.wps', 'r+').readlines()

nameListWPS[3]=start_date

nameListWPS[4] = end_date
nameListWPS[45]=" prefix = 'GFS',\n"
textTempWPS = open(path+'//namelist.wps', 'w+')
textTempWPS.writelines(nameListWPS)
textTempWPS.close()

cmdMetGridRun = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/metgrid.exe', '--arg', 'value']
exe_Run(cmdMetGridRun)

nameListInput = open(path+'//namelist.input', 'r+').readlines()

nameListInput[5]  = ' start_year                          = ' + forecastDate.strftime('%Y') +', '+ forecastDate.strftime('%Y')+', '+ forecastDate.strftime('%Y')+ ",\n"
nameListInput[6]  = ' start_month                         = ' + forecastDate.strftime('%m') +', '+ forecastDate.strftime('%m')+', '+ forecastDate.strftime('%m')+ ",\n"
nameListInput[7]  = ' start_day                           = ' + forecastDate.strftime('%d') +', '+ forecastDate.strftime('%d')+', '+ forecastDate.strftime('%d')+ ",\n"
nameListInput[8]  = ' start_hour                          = ' + forecastDate.strftime('%H') +', '+ forecastDate.strftime('%H')+', '+ forecastDate.strftime('%H')+ ",\n"
nameListInput[11]  = ' end_year                            = ' + (forecastDate+timedelta(timeDelta)).strftime('%Y') +', '+ (forecastDate+timedelta(timeDelta)).strftime('%Y')+', '+ (forecastDate+timedelta(timeDelta)).strftime('%Y')+ ",\n"
nameListInput[12]  = ' end_month                           = ' + (forecastDate+timedelta(timeDelta)).strftime('%m') +', '+ (forecastDate+timedelta(timeDelta)).strftime('%m')+', '+ (forecastDate+timedelta(timeDelta)).strftime('%m')+ ",\n"
nameListInput[13] = ' end_day                             = ' + (forecastDate+timedelta(timeDelta)).strftime('%d') +', '+ (forecastDate+timedelta(timeDelta)).strftime('%d')+', '+ (forecastDate+timedelta(timeDelta)).strftime('%d')+ ",\n"
nameListInput[14] = ' end_hour                            = ' + (forecastDate+timedelta(timeDelta)).strftime('%H') +', '+ (forecastDate+timedelta(timeDelta)).strftime('%H')+', '+ (forecastDate+timedelta(timeDelta)).strftime('%H')+ ",\n"

textTempInput = open(path+'//namelist.input', 'w+')
textTempInput.writelines(nameListInput)
textTempInput.close()

cmdReal = ['/home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/real.exe', '--arg', 'value']
exe_Run(cmdReal)
cmdWRFexe = ['/home/fmg-wrf/Build_WRF/LIBRARIES/mpich/bin/mpirun -np 16 /home/fmg-wrf/Build_WRF/WRF_Run/run_WRF/wrf.exe', '--arg', 'value']
exe_Run(cmdWRFexe)
#copyWRFtoFTP = ['cp wrfout_d01* /var/www/html/community/bwdb/wrfout_d01']
#exe_Run(copyWRFtoFTP)
copyfile("wrfout_d01_"+ (forecastDate).strftime('%Y-%m-%d_%H')+":00:00",   pathData+'//'+ forecastDate.strftime('%Y-%m-%d_%H') +"//wrfout_d01_"+ (forecastDate).strftime('%Y-%m-%d_%H')+":00:00")
#wrfout_d01_2019-11-24_00:00:00
os.remove("wrfout_d01_"+ (forecastDate).strftime('%Y-%m-%d_%H')+":00:00")
