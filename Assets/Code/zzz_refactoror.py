import os


f = []
for (dirpath, dirnames, filenames) in os.walk("."):
    f.extend(filenames)
    break

for file in f:
	if file[-3:] == ".cs":
		#print("Valid " + str(file))
		changed = False
		openFile = open(file,"r")
		lines = []
		for line in openFile.readlines():
			lines.append(line)
			#print(line)
		ind = -1
		for line in lines:
			ind += 1
			if " class " in line:
				if (":") in line:
					#Nothing, this class inherits from something
					print("CHECK THIS DOESN'T IMPLEMENT AN INTERFACE: " + line)
					pass
				else:
					newLine = line[:-1] + " : SerializedScriptableObject\n"
					print("Replacing " + line[:-1] + " with " + newLine)
					changed = True
					lines[ind] = newLine
		openFile.close()
		
		if (changed):
			lines.insert(0,"using OdinSerializer;\n")
			print("Writing to " + file)
			openFile = open(file,"w")
			for line in lines:
				openFile.write(line)
			openFile.close()
		