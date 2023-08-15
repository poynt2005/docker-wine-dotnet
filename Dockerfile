FROM scottyhardy/docker-wine:stable-8.0.1

ARG WINE_ARCH "win32"

ENV WINEDEBUG "fixme-all"

ENV RUN_AS_ROOT "yes"
ENV USE_XVFB "yes"
ENV XVFB_SERVER ":95"
ENV XVFB_SCREEN "0"
ENV XVFB_RESOLUTION "1024x768x8"
ENV DISPLAY ":95"
ENV WINEARCH $WINE_ARCH

RUN set -x -e; \
    entrypoint wineboot --init; \
    while true; do \
      if timeout 30m winetricks --unattended --force cmd dotnet20 dotnet472 dotnet48 d3dx9_43 corefonts; then \
        break; \
      fi \
    done; \
    while pgrep wineserver >/dev/null; do echo "Waiting for wineserver"; sleep 1; done; \
    rm -rf $HOME/.cache/winetricks;
