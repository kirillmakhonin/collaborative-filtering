

File.open('1-answer.csv').read.each_line do |line|
	user, item, rate = line.split(',').map { |i| i.to_i }
	
	mark_id = "new BaseEntities.Mark() { uri = \"mark/m#{user}_#{item}_#{rate}\" }";
	
	puts "rates.Add(new Tuple<BaseEntities.Mark, int>(#{mark_id}, #{rate}));\n" + \
	"items.Add(new Tuple<BaseEntities.Mark, int>(#{mark_id}, #{item}));\n" + \
	"users.Add(new Tuple<BaseEntities.Mark, int>(#{mark_id}, #{user}));\n" + \
	"uploadedBy.Add(new Tuple<BaseEntities.Mark,BaseEntities.Student>(#{mark_id}, me));\n\n";
	
end