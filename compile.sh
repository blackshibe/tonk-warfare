rm -rf build
mkdir build
mkdir build/client-linux
mkdir build/server-linux
echo "building client"
/mnt/mainstorage/unity/2021.2.16f1/Editor/Unity -quit -nographics -batchmode -logfile client.txt -projectPath client -buildLinux64Player "$(realpath .)/build/client-linux/tonk_warfare-linux.x86_64" -buildTarget Linux64
echo "building server"
/mnt/mainstorage/unity/2021.2.16f1/Editor/Unity -quit -nographics -batchmode -logfile server.txt -projectPath server -buildLinux64Player "$(realpath .)/build/server-linux/tonk_warfare-linux.x86_64" -buildTarget Standalone