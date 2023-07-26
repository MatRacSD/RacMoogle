

if [[ "$1" == "run" ]]; then
    if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
      powershell.exe -Command "cd MoogleServer; dotnet watch run"
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        open -a Terminal.app MoogleServer -n --args bash -c "dotnet watch run"
    elif [[ "$OSTYPE" == "linux-gnu" ]]; then
        gnome-terminal --working-directory="$PWD/MoogleServer" -x bash -c "dotnet watch run"
    fi
    sleep 5 
    if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
        start "http://localhost:5285"
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        open "http://localhost:5285"
    elif [[ "$OSTYPE" == "linux-gnu" ]]; then
        xdg-open "http://localhost:5285"
    fi

elif [[ "$1" == "report" ]]; then
    pdflatex informe.tex

elif [[ "$1" == "show_report" ]]; then
    if [[ -n "$2" ]]; then
        APP="$2"
    else
        if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
            APP="start"
        elif [[ "$OSTYPE" == "darwin"* ]]; then
            APP="open"
        elif [[ "$OSTYPE" == "linux-gnu" ]]; then
            APP="xdg-open"
        fi
    fi
    pdflatex informe.tex
    if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
        $APP "$(cygpath -w informe.pdf)"
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        $APP -a "$2" informe.pdf
    elif [[ "$OSTYPE" == "linux-gnu" ]]; then
        $APP informe.pdf
    fi

elif [[ "$1" == "slides" ]]; then
    pdflatex presentacion.tex


elif [[ "$1" == "show_slides" ]]; then
    if [[ -n "$2" ]]; then
        APP="$2"
    else
        if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
            APP="start"
        elif [[ "$OSTYPE" == "darwin"* ]]; then
            APP="open"
        elif [[ "$OSTYPE" == "linux-gnu" ]]; then
            APP="xdg-open"
        fi
    fi
    pdflatex presentacion.tex
    if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
        $APP "$(cygpath -w presentacion.pdf)"
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        $APP -a "$2" presentacion.pdf
    elif [[ "$OSTYPE" == "linux-gnu" ]]; then
        $APP presentacion.pdf
    fi

elif [[ "$1" == "clean" ]]; then
    find . -type f -regextype posix-extended -regex '.*\.(aux|bbl|fdb_latexmk|fls|pdf|gz|toc|log|nav|out|snm)' -delete
    find . -name ".vscode" -type d -exec rm -rf {} +
    find . -type d -name "bin" -o -name "obj" -exec rm -rf {} +
fi