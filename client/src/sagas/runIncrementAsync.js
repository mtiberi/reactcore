import { put, takeEvery } from 'redux-saga/effects'

const delay = ms => new Promise(res => setTimeout(res, ms))

export default function runIncrementAsync () {

  return function *() {
    yield takeEvery('INCREMENT_ASYNC', run)
  }

  function *run () {
    yield put({ type: 'INCREMENT' })
    yield delay(1000)
    yield put({ type: 'DECREMENT' })
  }

}
