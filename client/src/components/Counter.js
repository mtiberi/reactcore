import React from 'react'
import { connect } from 'react-redux'

const Counter = ({ value, increment, decrement, incrementAsync }) => (
  <div>
    <button onClick={incrementAsync}>Increment after 1 second</button>{' '}
    <button onClick={increment}>Increment</button>{' '}
    <button onClick={decrement}>Decrement</button>
    <hr />
    <div>Clicked: {value} times</div>
  </div>
)

const increment = () => ({ type: 'INCREMENT' })
const decrement = () => ({ type: 'DECREMENT' })
const incrementAsync = () => ({ type: 'INCREMENT_ASYNC' })

const mapStateToProps = state => ({
  value: state.value
})

export default connect(
  mapStateToProps,
  {
    increment,
    decrement,
    incrementAsync
  }
)(Counter)
