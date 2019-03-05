const fs = require('fs')
const path = require('path')
const yarnjs = require('./yarn-1.14.0')

const yarnAt = directory => {
  const startArgs = ['node', 'yarn']
  const endArgs = []
  yarnjs.main({
    startArgs,
    args: ['-v', '--cwd', directory],
    endArgs
  })
  return function () {
    const args = ['--cwd', directory, '--non-interactive', ...arguments]
    console.log('yarn', ...args)
    return yarnjs.main({
      startArgs,
      args,
      endArgs
    })
  }
}

function mostRecent (entry) {
  if (!fs.existsSync(entry)) return -1

  const stat = fs.statSync(entry)

  if (stat.isFile()) return stat.mtime.getTime()

  if (stat.isDirectory()) {
    let max = stat.mtime.getTime()
    fs.readdirSync(entry)
      .filter(file => file !== 'node_modules')
      .forEach(file => {
        const fullPath = path.join(entry, file)
        const next = mostRecent(fullPath)
        max = next > max ? next : max
      })
    return max
  }

  return -1
}

function removeSync (item) {
  if (!fs.existsSync(item)) return

  var stat = fs.lstatSync(item)
  if (stat.isFile()) return fs.unlinkSync(item)
  if (stat.isDirectory()) {
    fs.readdirSync(item).forEach(entry => removeSync(path.join(item, entry)))
    fs.rmdirSync(item)
  }
}

function copySync (from, to) {
  var stat = fs.lstatSync(from)
  if (stat.isFile()) return fs.copyFileSync(from, to)
  if (stat.isDirectory()) {
    if (!fs.existsSync(to)) fs.mkdirSync(to)

    fs.readdirSync(from).forEach(entry => {
      const s = path.join(from, entry)
      const d = path.join(to, entry)
      copySync(s, d)
    })
  }
}

async function build () {
  const targetDir = path.resolve(process.argv[3])
  const sourceDir = path.resolve(process.argv[2])

  const targetTime = mostRecent(targetDir)
  const sourceTime = mostRecent(sourceDir)

  if (fs.existsSync(targetDir) && targetTime >= sourceTime) {
    console.log('no need to rebuild client')
  } else {
    console.log('rebuild client')

    removeSync(path.join(sourceDir, 'build'))
    removeSync(targetDir)
    fs.mkdirSync(targetDir)

    const yarn = yarnAt(sourceDir)

    await yarn('install', '--prefer-offline')
    await yarn('build')

    copySync(path.join(sourceDir, 'build'), targetDir)
  }
  console.log('wwwroot -> ', targetDir)
}

build()
  .then(() => process.exit())
  .catch(err => console.log(err))
