import urllib.request
from PIL import Image
from pyzbar.pyzbar import decode
import io

def downloadFile(url):
    response = urllib.request.urlopen(url)
    return response.read()

def saveToFile(filename, content):
    with open(filename, 'wb') as f:
        f.write(content)

# s = '75198582,^517,-802,-420,*717,+555,+519,+315'
def evalOperations(s):
    print('eval('+s+')')
    v = eval('(' * s.count(',') + s.replace(',', ')'))    
    return v

def urlQRCode(s):
    gyn = 'http://gynvael.coldwind.pl/qrmaze/'
    return gyn + s + '.png'
        
def decodeQR(pngFileContent):
    decoded = decode(Image.open(io.BytesIO(pngFileContent)))  
    return decoded[0].data
  
fname = 'start'
c = 0
while True:
    url = urlQRCode(fname)
    
    print('Downloading('+str(c)+'): '+url)
    filecontent = downloadFile(url)
    # saveToFile('qrcodes/'+calc+'.png', filecontent)
    
    qrd = decodeQR(filecontent).decode()   
    # qrd = "Calc value, add .png, repeat: 64038932,+145,*881,+493,*746"
     
    rpos = qrd.find('repeat:')    
    fname = str(evalOperations(qrd[rpos+8:]))

    c += 1

