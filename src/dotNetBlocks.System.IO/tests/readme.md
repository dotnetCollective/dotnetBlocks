# Testing notes

# Hanging tests.

When designing the tests, remember the reader will block until the writer is closed or until there is something to ready. This is fine if you are reading and writing chunks in the testing.
It becomes a problem when one test e.g. CRC wants to continue reading until the write stream is closed. You can easily design a test to deadlock yourself.

The CRC method that reads an entire stream can easily deadlock your testing, so be aware!!