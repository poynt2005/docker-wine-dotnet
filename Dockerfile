FROM poynt2005/docker-wine-dotnet:dotnet48-wine96-devel AS dotnet-base

RUN apt update
RUN apt install imagemagick xdotool -y 

COPY xvfb_checker.py change_win_version.py webview2.exe ./

COPY try_install.py ./

RUN python3 try_install.py