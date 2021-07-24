BASEPATH=$1
SSH=$2
SERVERPATH=ACContentSynchronizer.Server
DEPLOYPATH=$BASEPATH/$SERVERPATH
ARCHIVE=$SERVERPATH.zip
echo BASEPATH=$BASEPATH
echo SERVERPATH=$SERVERPATH
echo ARCHIVE=$ARCHIVE
echo SSH=$SSH
rm -rf $DEPLOYPATH
dotnet publish -c Release -o $DEPLOYPATH -r win-x64 -p:PublishSingleFile=true --self-contained true
cd $BASEPATH;
rm $ARCHIVE
tar.exe -acf $ARCHIVE $SERVERPATH
scp $ARCHIVE $SSH:;
ssh $SSH "echo $SERVERPATH &\
sc.exe stop $SERVERPATH &\
echo Unpack &\
tar -xvf $ARCHIVE &&\
echo Move &\
cd $SERVERPATH &&\
echo Directory &\
mkdir \"C:/$SERVERPATRH\" &\
echo Copy &\
copy /Y \"$SERVERPATH.exe\" \"C:/$SERVERPATH/$SERVERPATH.exe\" &&\
copy /Y \"aspnetcorev2_inprocess.dll\" \"C:/$SERVERPATH/aspnetcorev2_inprocess.dll\" &&\
copy /Y \"clrcompression.dll\" \"C:/$SERVERPATH/clrcompression.dll\" &&\
copy /Y \"clrjit.dll\" \"C:/$SERVERPATH/clrjit.dll\" &&\
copy /Y \"coreclr.dll\" \"C:/$SERVERPATH/coreclr.dll\" &&\
copy /Y \"mscordaccore.dll\" \"C:/$SERVERPATH/mscordaccore.dll\" &&\
copy /Y \"web.config\" \"C:/$SERVERPATH/web.config\" &&\
echo Create &\
sc.exe create $SERVERPATH binpath=\"C:/$SERVERPATH/$SERVERPATH.exe\" &\
echo Start &\
sc.exe start $SERVERPATH"
