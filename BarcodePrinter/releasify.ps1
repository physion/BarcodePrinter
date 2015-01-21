# Build nuget package
nuget pack;

# Build release
Squirrel --releasify=PhysionBarcodePrinter.1.0.0.0.nupkg --packagesDir=../packages; #--signWithParams=""