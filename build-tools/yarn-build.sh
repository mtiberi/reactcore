scriptDir=$(dirname "$(readlink -f "$0")")
export PATH="$scriptDir:$PATH"
node "$scriptDir/buildClient.js" "$1" "$2"
