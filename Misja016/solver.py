# Invert Look-Up Table
def InvertLUT(LUT):    
    iLUT = [0] * 256    
    for i in range(0, 256):        
        f = 0
        while (LUT[f] != i):
            f += 1
        iLUT[i] = f        
    return iLUT

# Decoding right channel of the wave form off using LUT
def Decode(wav, off, LUT):
    iLUT = InvertLUT(LUT)
    p = 0
    for i in range(off, len(wav) - off):
        if ((p & 2) != 0):
            if ((p & 0x80) != 0):
                wav[i] = iLUT[wav[i]]
            wav[i] = iLUT[wav[i]]
        p += 1
 
# Read LUT table from stream.bmp
# It's hidden in the alpha channel of the palette
with open("stream.bmp", "rb") as f:
    st = f.read()
    
LUT = []
for i in range(0, 256):
    LUT.append(st[54+3 + i*4])    

# Decoding...
with open("wave_from_stream.wav", "rb") as f:
    wav = bytearray(f.read())

Decode(wav, 40, LUT)

with open("solved.wav", "wb") as f:
    f.write(bytes(wav))

