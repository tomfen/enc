import pandas
from nltk import word_tokenize, PorterStemmer, re
from nltk.corpus import reuters, stopwords
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.preprocessing import MultiLabelBinarizer
from sklearn.svm import SVC
from sklearn.multiclass import OneVsRestClassifier
from sklearn.metrics import f1_score

cachedStopWords = stopwords.words("english")

def tokenize(text):
    min_length = 3

    words = word_tokenize(text)
    words = [word.lower() for word in words if word not in cachedStopWords]
    tokens = (list(map(lambda token: PorterStemmer().stem(token), words)))

    p = re.compile('[a-zA-Z]+')
    filtered_tokens = list(filter(lambda token: p.match(token) and len(token) >= min_length, tokens))

    return filtered_tokens

train = []
train_ids = []
train_cat = []
test = []
test_ids = []
test_cat = []

classes = ['earn', 'acq', 'money-fx', 'grain', 'crude', 'trade', 'interest', 'ship', 'wheat', 'corn']

for doc in reuters.fileids():
    if doc.startswith("train"):
        train.append(reuters.raw(doc))
        train_ids.append(doc)
        train_cat.append(set(reuters.categories(doc)).intersection(classes))
    elif doc.startswith("test"):
        test.append(reuters.raw(doc))
        test_ids.append(doc)
        test_cat.append(set(reuters.categories(doc)).intersection(classes))

vectorizer = TfidfVectorizer(max_features=10000, tokenizer=tokenize, use_idf=False, norm='l1')
label_binarizer = MultiLabelBinarizer(classes=classes)

vectorised_train_documents = vectorizer.fit_transform(train)
vectorised_test_documents = vectorizer.transform(test)

binarized_train_labels = label_binarizer.fit_transform(train_cat)
binarized_test_labels = label_binarizer.transform(test_cat)

binarized_train_labels[binarized_train_labels == 0] = -1
binarized_test_labels[binarized_test_labels == 0] = -1

dftrain = pandas.DataFrame(vectorised_train_documents.todense(), index=train_ids, columns=vectorizer.vocabulary_)
dftrainlabels = pandas.DataFrame(binarized_train_labels, index=train_ids, columns=classes)
dftrain.join(dftrainlabels, rsuffix="_cat").to_csv('train.csv', header=True, index=False)

dftest = pandas.DataFrame(vectorised_test_documents.todense(), index=test_ids, columns=vectorizer.vocabulary_)
dftestlabels = pandas.DataFrame(binarized_test_labels, index=test_ids,columns=classes)
dftest.join(dftestlabels, rsuffix="_cat").to_csv('test.csv', header=True, index=False)

# svm = OneVsRestClassifier(SVC(C=1000))
# svm.fit(vectorised_train_documents, binarized_train_labels)
# YY = svm.predict(vectorised_test_documents)
# f1_micro = f1_score(binarized_test_labels, YY, average='micro')
# print(f1_micro)

