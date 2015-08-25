require 'zip'

# This packs all content into a big zip aka content-pack
class Packer
  def self.pack(pack_name)
    zipfile = File.expand_path "public/#{pack_name}.zip"
    
    # delete the old zip
    File.delete zipfile if File.exists? zipfile

    # content directory
    dir = File.expand_path("content/#{pack_name}") + '/'

    Zip::File.open(zipfile, Zip::File::CREATE) do |zipfile|
      Dir[File.join(dir, '**', '**')].each do |file|
        # no ruby files
        next if file =~ /.rb$/
        
        # not the event folder
        next if file.end_with? "events"
        
        # add the file
        zipfile.add(file.sub(dir, ''), file)
      end
    end
  end
end