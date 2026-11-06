@echo off
REM =====================================================================
REM  BISAYA: HIMO OG APK (Android) - i-double-click lang kini nga file.
REM   1. Kung wala pa ang fsi.keystore (pirma sa app): himuon, mangayo og password (KAUSA RA).
REM   2. Ibuild ang Release APK nga naay pirma.
REM   3. Ablihan ang folder nga naay .apk.
REM  HINUMDOMI: ang fsi.keystore ug ang password ayaw iwala - kinahanglan sa matag update.
REM  Sa matag bag-ong APK: +1 sa <ApplicationVersion> sa fsi checklist.csproj.
REM =====================================================================
setlocal
cd /d "%~dp0"

REM --- pangitaa ang keytool (kauban sa JDK nga gi-install sa Visual Studio) ---
set "KEYTOOL="
where keytool >nul 2>nul && set "KEYTOOL=keytool"
if not defined KEYTOOL for /d %%D in ("C:\Program Files\Microsoft\jdk-*") do if exist "%%D\bin\keytool.exe" set "KEYTOOL=%%D\bin\keytool.exe"
if not defined KEYTOOL for /d %%D in ("C:\Program Files\Android\openjdk\jdk-*") do if exist "%%D\bin\keytool.exe" set "KEYTOOL=%%D\bin\keytool.exe"
if not defined KEYTOOL if exist "C:\Program Files\Android\Android Studio\jbr\bin\keytool.exe" set "KEYTOOL=C:\Program Files\Android\Android Studio\jbr\bin\keytool.exe"

echo.
echo ===== BFP Fire Safety Checklist: HIMO OG APK =====
echo.
set /p "PASS=Password sa keystore (bag-o kung una, o ang daan): "
if "%PASS%"=="" ( echo Walay password. & pause & exit /b 1 )

if not exist "fsi.keystore" (
    if not defined KEYTOOL (
        echo Wala makit-an ang keytool. I-install ang Android workload sa Visual Studio Installer.
        pause & exit /b 1
    )
    echo.
    echo [1/3] Himoon ang fsi.keystore ...
    "%KEYTOOL%" -genkeypair -keystore fsi.keystore -alias fsi -keyalg RSA -keysize 2048 -validity 10000 -storepass "%PASS%" -keypass "%PASS%" -dname "CN=Prince Rainier A. Datoy, O=BFP, L=Tangub City, C=PH"
    if errorlevel 1 ( echo Napakyas ang paghimo sa keystore. & pause & exit /b 1 )
) else (
    echo [1/3] Naa nay fsi.keystore - gamiton kini.
)

echo.
echo [2/3] Ibuild ang APK (5-10 minutos sa una) ...
dotnet publish "fsi checklist.csproj" -f net10.0-android -c Release -m:1 -p:AndroidPackageFormat=apk -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=fsi.keystore -p:AndroidSigningKeyAlias=fsi -p:AndroidSigningKeyPass="%PASS%" -p:AndroidSigningStorePass="%PASS%"
if errorlevel 1 ( echo. & echo NAPAKYAS ang build. I-screenshot ang pula nga error. & pause & exit /b 1 )

echo.
echo [3/3] HUMAN! Ang APK: bin\Release\net10.0-android\publish\com.bfp.fsichecklist-Signed.apk
explorer "bin\Release\net10.0-android\publish"
pause
