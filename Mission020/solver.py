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
# return: '75198541'
def evalOperations(s):
    s = '(' * s.count(',') + s.replace(',', ')')
    v = str(eval(s))
    print('  eval('+s+') = ' + v)
    return v

def urlQRCode(s):
    gyn = 'http://gynvael.coldwind.pl/qrmaze/'
    return gyn + s + '.png'
        
def decodeQR(pngFileContent):
    # Decode QRCode from the PNG image
    decoded = decode(Image.open(io.BytesIO(pngFileContent)))  
    # decoded = [Decoded(data=b'Calc value, add .png, repeat: 80791583,*615,^381', type='QRCODE')]
    return decoded[0].data.decode()
  
fname = 'start'
c = 0
while True:
    url = urlQRCode(fname)
    
    print('Downloading('+str(c)+'): '+url)
    filecontent = downloadFile(url)
    # saveToFile('qrcodes/'+calc+'.png', filecontent)
    
    qrd = decodeQR(filecontent)   
    # qrd = "Calc value, add .png, repeat: 64038932,+145,*881,+493,*746"
     
    rpos = qrd.find('repeat:')    
    fname = evalOperations(qrd[rpos+8:])

    c += 1

