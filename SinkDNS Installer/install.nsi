; SinkDNS Installer Script
; Requires: DNSCryptProxy\, SinkDNS\, DNSCryptConfig\, Credits.txt in same folder as this .nsi

; TODO: Test on VM, and on install fail, run uninstall section
;Also TODO: if possible, also install .Net Desktop Runtime version 10.0.5 x64 since install now works but running SinkDNS doesn't on a fresh Windows OS.

!include "MUI2.nsh"
!include "LogicLib.nsh"

Name "SinkDNS"
OutFile "SinkDNS-Setup.exe"
RequestExecutionLevel admin

InstallDir "$LOCALAPPDATA\SinkDNS"

!define DNSCRYPT_DIR "$PROGRAMFILES64\dnscrypt-proxy"

;--------------------------------
; MUI Pages
;--------------------------------
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "Credits.txt"
!insertmacro MUI_PAGE_COMPONENTS
;!insertmacro MUI_PAGE_DIRECTORY - Removed
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

;--------------------------------
; Check for running dnscrypt-proxy service
;--------------------------------
Function .onInit
    ExecWait 'cmd /C sc query "dnscrypt-proxy" | find "RUNNING" >nul' $0
    ${If} $0 = 0
        MessageBox MB_ICONEXCLAMATION|MB_OKCANCEL \
        "A running dnscrypt-proxy service was detected.$\r$\n$\r$\n\
        You should uninstall dnscrypt-proxy before installing SinkDNS.$\r$\n$\r$\n\
        Press OK to continue anyway, or Cancel to abort." \
        IDOK continue
        Abort
    continue:
    ${EndIf}
FunctionEnd

;--------------------------------
; Optional Components
;--------------------------------

Section "Start SinkDNS on Startup" SecStartup
SectionEnd

Section "Add Start Menu Shortcut" SecStartMenu
SectionEnd

Section "Add Desktop Shortcut" SecDesktop
SectionEnd

;--------------------------------
; Main Install Section
;--------------------------------
Section "Install SinkDNS"

    ; Create dnscrypt-proxy folder
    CreateDirectory "${DNSCRYPT_DIR}"
    SetOutPath "${DNSCRYPT_DIR}"
    File /r "DNSCryptProxy\*"

    ; Install SinkDNS to LocalAppData
    CreateDirectory "$INSTDIR"
    SetOutPath "$INSTDIR"
    File /r "SinkDNS\*"

    ; Copy dnscrypt-proxy.toml (force overwrite)
    SetOutPath "$TEMP"
    File "DNSCryptConfig\dnscrypt-proxy.toml"

    Delete "${DNSCRYPT_DIR}\dnscrypt-proxy.toml"
    CopyFiles /SILENT "$TEMP\dnscrypt-proxy.toml" "${DNSCRYPT_DIR}\dnscrypt-proxy.toml"
    Delete "$TEMP\dnscrypt-proxy.toml"

    ; Install dnscrypt-proxy service
    ExecWait '"${DNSCRYPT_DIR}\dnscrypt-proxy.exe" -service install' $0
    ${If} $0 != 0
        MessageBox MB_ICONSTOP "Failed to install dnscrypt-proxy service. Aborting."
        Abort
    ${EndIf}

    ; Stop service (We want SinkDNS to configure DNSCrypt-Proxy.)
    ExecWait 'cmd /C sc stop "dnscrypt-proxy"' $1
    Sleep 1500

    ; Write uninstall entry
    WriteUninstaller "$INSTDIR\Uninstall.exe"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\SinkDNS" "DisplayName" "SinkDNS"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\SinkDNS" "UninstallString" "$INSTDIR\Uninstall.exe"

    ;--------------------------------
    ; Optional: Start SinkDNS on Startup
    ;--------------------------------
    SectionGetFlags ${SecStartup} $0
    IntOp $0 $0 & ${SF_SELECTED}
    ${If} $0 <> 0
        WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "SinkDNS" '"$INSTDIR\SinkDNS.exe"'
    ${EndIf}

    ;--------------------------------
    ; Optional: Start Menu Shortcut
    ;--------------------------------
    SectionGetFlags ${SecStartMenu} $0
    IntOp $0 $0 & ${SF_SELECTED}
    ${If} $0 <> 0
        CreateDirectory "$SMPROGRAMS\SinkDNS"
        CreateShortcut "$SMPROGRAMS\SinkDNS\SinkDNS.lnk" "$INSTDIR\SinkDNS.exe"
    ${EndIf}

    ;--------------------------------
    ; Optional: Desktop Shortcut
    ;--------------------------------
    SectionGetFlags ${SecDesktop} $0
    IntOp $0 $0 & ${SF_SELECTED}
    ${If} $0 <> 0
        CreateShortcut "$DESKTOP\SinkDNS.lnk" "$INSTDIR\SinkDNS.exe"
    ${EndIf}

SectionEnd

;--------------------------------
; Uninstaller
;--------------------------------
Section "Uninstall"

    ; Check if SinkDNS.exe is running
    ExecWait 'cmd /C tasklist /FI "IMAGENAME eq SinkDNS.exe" | find "SinkDNS.exe" >nul' $0
    ${If} $0 = 0
        MessageBox MB_ICONSTOP "SinkDNS.exe is currently running.$\r$\n\
Please close SinkDNS before uninstalling." 
        Abort
    ${EndIf}

    ; Stop dnscrypt-proxy service if running
    ExecWait 'cmd /C sc stop "dnscrypt-proxy" >nul' $1
    Sleep 1000

    ; Uninstall dnscrypt-proxy service
    ExecWait '"${DNSCRYPT_DIR}\dnscrypt-proxy.exe" -service uninstall' $2

    ; Remove dnscrypt-proxy folder
    RMDir /r "${DNSCRYPT_DIR}"

    ; Remove SinkDNS folder
    RMDir /r "$INSTDIR"
    
    ; Remove startup entry
    DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "SinkDNS"

    ; Remove Start Menu folder
    RMDir /r "$SMPROGRAMS\SinkDNS"

    ; Remove Desktop shortcut
    Delete "$DESKTOP\SinkDNS.lnk"

    ; Remove registry entries
    DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\SinkDNS"

SectionEnd