if [[ "$OSTYPE" == "darwin"* ]]; then
  source ~/.zprofile
    fi
$JAVA_HOME/bin/keytool -genkey -noprompt -alias $1 -dname "CN=$6, OU=$5, O=$5, L=$7, S=$8, C=$8" -keystore $2 -storepass $3 -keypass $4 -keyalg RSA -keysize 2048 -validity 10000
