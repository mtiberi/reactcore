
child_process = require('child_process')

function run (processName, ...args) {
  const child = child_process.spawn(processName, args)
  child.stdout.pipe(process.stdout)
  child.on('exit', () => process.exit())
  child.on('error', error => {
    console.log(error)
    process.exit(1)
  })
}

process.env.PORT=process.argv[2]
process.chdir('../server/bin/Debug/netcoreapp2.0')
run('dotnet','reactcore.dll')
