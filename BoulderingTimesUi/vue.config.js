module.exports = {
    devServer: {
        proxy: {
            '^/boulderingtimes': {
                target: process.env.VUE_APP_API_URL
            }
        },
        port: 5002
    }
}