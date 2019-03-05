import React from 'react'
import ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import store from 'store'

import 'normalize.css'
import App from 'components/App'
import config from 'assets/config'

// set title and icon
(function() {
    document.title = config.title
    const link = document.createElement('link')
    link.rel = 'shortcut icon'
    link.href = config.icon
    document.head.appendChild(link)
})()

ReactDOM.render(
    <Provider store={store} >
        <App />
    </Provider>,
    document.getElementById('root')
)
