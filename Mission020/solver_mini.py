import urllib.request
from PIL import Image
from pyzbar.pyzbar import decode
import io

fname = 'start'
c = 0
while True:
    url = 'http://gynvael.coldwind.pl/qrmaze/' + fname + '.png'
    print('Downloading('+str(c)+'): '+url)
    
    filecontent = urllib.request.urlopen(url).read()
    
    qrd = decode(Image.open(io.BytesIO(filecontent)))[0].data.decode()   
    # qrd = "Calc value, add .png, repeat: 64038932,+145,*881,+493,*746"
    
    math = qrd[qrd.find('repeat:')+8:]
    v = eval('(' * math.count(',') + math.replace(',', ')'))
    
    fname = str(v)
    c += 1
