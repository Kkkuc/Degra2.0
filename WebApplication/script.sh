#!/bin/bash

directories=("Controllers" "Data" "Models" "Services" "Views")

for dir in "${directories[@]}"; do
    if [ -d "$dir" ]; then
        find "$dir" -type f | while read -r file; do
            echo "--- File: $file ---"
            cat "$file"
            echo -e "\n"
        done >> output.txt
    fi
done
