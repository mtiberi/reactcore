import runIncrementAsync from './runIncrementAsync'

const processes = [
  runIncrementAsync
]

export default function (sagas) {
  processes.forEach(item => sagas.run(item()))
}
