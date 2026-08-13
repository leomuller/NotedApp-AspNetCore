# Copilot Instructions

## Project Guidelines
- Always provide code with explicit curly braces { } for control structures and blocks; avoid code without braces.
- Avoid the negative operator (e.g., !not); prefer explicit if/else or comparisons like == false.
- Do not create inline classes like connection factories inside other files; define them as separate, named types.
- Avoid inline classes like NotedConnectionFactory inside Program.cs; keep Program.cs short and focused; do not put every property there.
- Prefer registering services through extension methods in the relevant project.
- Always check the last/current version of a file before suggesting changes to it.

## Code Formatting
- Write the file name above every code block, unless the block is not intended for the codebase.
- Always write the file name above code blocks in responses.
- Split code across multiple lines for readability; avoid condensing multiple conditions or assignments into a single line.