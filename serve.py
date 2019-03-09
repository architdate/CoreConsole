from waitress import serve
import logging
from paste.translogger import TransLogger
import server

logger = logging.getLogger('waitress')
logger.setLevel(logging.DEBUG)
serve(TransLogger(server.app, setup_console_handler=True), host='127.0.0.1', port=5000,)
