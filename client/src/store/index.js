import { createStore, applyMiddleware, compose } from 'redux'
import createSagaMiddleware from 'redux-saga'
import runSagas from 'sagas'

let store

const initialState = {
  value: 0
}

const updateState = (state, diff) => ({ ...state, ...diff })

const increment = state => updateState(state, { value: state.value + 1 })

const decrement = state => updateState(state, { value: state.value - 1 })

const reducerMap = {
  INCREMENT: increment,
  DECREMENT: decrement
}

const noHandler = state => state

function reducer (state = {}, action) {
  const handler = reducerMap[action.type] || noHandler
  return handler(state, action.payload)
}

function addDevTools (...middleware) {
  const devtools_compose = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__
  const composeEnhancers = devtools_compose ? devtools_compose({}) : compose
  return composeEnhancers(applyMiddleware(...middleware))
}

const sagaMiddleware = createSagaMiddleware()
const enhancer = addDevTools(sagaMiddleware)

store = createStore(reducer, initialState, enhancer)
runSagas(sagaMiddleware)

export default store
