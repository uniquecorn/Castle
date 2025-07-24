if [[ "$OSTYPE" == "darwin"* ]]; then
  source ~/.zprofile
    fi
kill $(ps aux | grep 'Xcode' | awk '{print $2}') || true
targetDirectory=$1
xcodeApp="Xcode"
didFindXCWorkspace=`find $targetDirectory -depth 1 | grep -i .xcworkspace | wc -l | sed -e 's/^ *//g' -e 's/ *$//g'`
if [ $didFindXCWorkspace == 1 ]; then
	open -a $xcodeApp $targetDirectory/*.xcworkspace
else
	didFindXcodeproj=`find $targetDirectory -depth 1 | grep -i .xcodeproj | wc -l | sed -e 's/^ *//g' -e 's/ *$//g'`
	if [ $didFindXcodeproj == 1 ]; then
		open -a $xcodeApp $targetDirectory/*.xcodeproj
	fi
fi