import getopt
import pandas
import sys
from nltk import word_tokenize, re, SnowballStemmer
from nltk.corpus import reuters, stopwords
from sklearn.decomposition import TruncatedSVD
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.preprocessing import MultiLabelBinarizer, MinMaxScaler
from sklearn.svm import SVC
from sklearn.multiclass import OneVsRestClassifier
from sklearn.metrics import f1_score


# Defaults
use_lsa = False
use_idf = False
max_features = None
components = 100
feature_min = -1
out_dir = '\\'

opts, args = getopt.getopt(sys.argv[1:], 'f:c:m:', ['lsa', 'idf', 'out='])
for o, a in opts:
    if o == "-f":
        max_features = int(a)
    elif o == '-c':
        components = int(a)
    elif o == '-m':
        feature_min = int(a)
    elif o == '--lsa':
        use_lsa = True
    elif o == '--idf':
        use_idf = True
    elif o == '--out':
        out_dir = a
    else:
        assert False, "unhandled option"
		
cachedStopWords = stopwords.words('english')


def tokenize(text):
    min_length = 3
    stemmer = SnowballStemmer('english')

    words = word_tokenize(text)
    stems = [stemmer.stem(word) for word in words if word not in cachedStopWords]

    p = re.compile('[a-zA-Z]+')
    filtered_tokens = list(filter(lambda token: p.match(token) and len(token) >= min_length, stems))

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

vectorizer = TfidfVectorizer(max_features=max_features, tokenizer=tokenize, use_idf=use_idf, norm='l1')
label_binarizer = MultiLabelBinarizer(classes=classes)

vectorised_train_documents = vectorizer.fit_transform(train)
vectorised_test_documents = vectorizer.transform(test)

binarized_train_labels = label_binarizer.fit_transform(train_cat)
binarized_test_labels = label_binarizer.transform(test_cat)

binarized_train_labels[binarized_train_labels == 0] = -1
binarized_test_labels[binarized_test_labels == 0] = -1

if use_lsa:
    lsa = TruncatedSVD(components)
    vectorised_train_documents = lsa.fit_transform(vectorised_train_documents)
    vectorised_test_documents = lsa.transform(vectorised_test_documents)
    feature_names = ['component {}'.format(x) for x in range(lsa.n_components)]
else:
    vectorised_train_documents = vectorised_train_documents.todense()
    vectorised_test_documents = vectorised_test_documents.todense()
    feature_names = vectorizer.vocabulary_

print('Wczytano {} cech.'.format(len(feature_names)))

# Normalizacja cech
scaler = MinMaxScaler(feature_range=(feature_min, 1), copy=False)
scaler.fit_transform(vectorised_train_documents)
scaler.transform(vectorised_test_documents)

# Zapisywanie danych
print("Zapisywanie zbioru treningowego")
DFTrain = pandas.DataFrame(vectorised_train_documents, index=train_ids, columns=feature_names)
DFTrainLabels = pandas.DataFrame(binarized_train_labels, index=train_ids, columns=classes)
DFTrain.join(DFTrainLabels, rsuffix="_cat").to_csv('train.csv', header=True, index=False)

print("Zapisywanie zbioru testowego")
DFTest = pandas.DataFrame(vectorised_test_documents, index=test_ids, columns=feature_names)
DFTestLabels = pandas.DataFrame(binarized_test_labels, index=test_ids, columns=classes)
DFTest.join(DFTestLabels, rsuffix="_cat").to_csv('test.csv', header=True, index=False)
