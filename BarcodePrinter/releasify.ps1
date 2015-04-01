# Build release
Squirrel --releasify=bin\debug\PhysionBarcodePrinter.1.1.0.1.nupkg --packagesDir=../packages --signWithParams="/t http://timestamp.digicert.com /a"
