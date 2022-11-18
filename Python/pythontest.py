def Find_DividerInBuffer ( buffer, search ) :
    pos1 = 0
    pos2 = 0
    pos3 = 0
    pos4 = 0
    asfound = False;
    aspos=0
    beginpos=0
    # convert buffer and search terms to uppercase
    buffer = buffer.upper ( )
    search = search.upper ( )
    pos1 = buffer.find ( search )
    # find AS wth trailing space

    print ( f"Looking for 'AS ' into pos1" )
    pos1 = buffer.find ( 'AS ', )
    if (pos1 == -1) :
        # no, try with leading space
        print ( f"Looking for ' AS' into pos2" )
        pos2 = buffer.find ( " AS", )
        if(pos2 >=0):
            asfound = True
            aspos = pos2
            print ( f"AS found at position {pos2}" )
        else:
            # no, try trailing CR/LF
            print ( f"Looking for 'AS\n' into pos3" )
            pos3 = buffer.find ( "AS\n" )
            if(pos3 >=0):
                asfound = True
                aspos = pos3
                print ( f"AS found at position {pos3}" )
            else:
                # no, try leading CR/LF
                print ( f"Looking for '\nAS' into pos4" )
                pos4 = buffer.find ( "\nAS" )
                if (pos4 >= 0):
                    asfound = True
                    aspos = pos4
                    print ( f"AS found at position {pos4}" )
                else:
                    return -1
    else:
        print ( f"AS found at position {pos1}" )
        aspos = pos1
        asfound=True

    print ( f"Looking for ' BEGIN' into pos1" )
    pos1 = buffer.find ( " BEGIN", )
    if (pos1 >= 0) :
        beginfound = True
        beginpos = pos1
        print ( f"BEGIN found at position {pos1}" )
    else:
        print ( f"Looking for 'BEGIN ' into pos2" )
        pos2 = buffer.find ( "BEGIN ", )
        if(pos2 >= 0):
            beginfound = True
            beginpos = pos2
        else:
            print ( f"Looking for 'BEGIN\n' into pos2" )
            pos3 = buffer.find ( "BEGIN\n", )
            if(pos3 >= 0):
                beginfound = True
                beginpos = pos2
                print ( f"BEGIN\n found at position {pos3}" )
            else:
                pos4 = buffer.find ( "\nBEGIN", )
                if(pos4 >= 0):
                    beginfound = True
                    beginpos = pos4
                    print ( f"\nBEGIN found at position {pos4}" )
                else:
                    beginpos = -1

    if (beginpos - aspos < 10) :
        print ( f"AS found at {aspos} which is within 10 characters of Begin at {beginpos}\nso a match has been found" )
        return (aspos, beginpos)
    else :
        print ( f"No match has been found" )
        return (aspos, beginpos)
