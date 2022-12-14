#
# Autogenerated by Thrift Compiler (0.9.2)
#
# DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
#
#  options string: py
#

from thrift.Thrift import TType, TMessageType, TException, TApplicationException
from ttypes import *
from thrift.Thrift import TProcessor
from thrift.transport import TTransport
from thrift.protocol import TBinaryProtocol, TProtocol
try:
  from thrift.protocol import fastbinary
except:
  fastbinary = None


class Iface:
  def checkUserAge(self, carrier, sessionId, verifier, standardAge):
    """
    Parameters:
     - carrier
     - sessionId
     - verifier
     - standardAge
    """
    pass

  def checkUserAgeWithDocomo(self, openIdRedirectUrl, standardAge, verifier):
    """
    Parameters:
     - openIdRedirectUrl
     - standardAge
     - verifier
    """
    pass

  def retrieveOpenIdAuthUrlWithDocomo(self):
    pass

  def retrieveRequestToken(self, carrier):
    """
    Parameters:
     - carrier
    """
    pass


class Client(Iface):
  def __init__(self, iprot, oprot=None):
    self._iprot = self._oprot = iprot
    if oprot is not None:
      self._oprot = oprot
    self._seqid = 0

  def checkUserAge(self, carrier, sessionId, verifier, standardAge):
    """
    Parameters:
     - carrier
     - sessionId
     - verifier
     - standardAge
    """
    self.send_checkUserAge(carrier, sessionId, verifier, standardAge)
    return self.recv_checkUserAge()

  def send_checkUserAge(self, carrier, sessionId, verifier, standardAge):
    self._oprot.writeMessageBegin('checkUserAge', TMessageType.CALL, self._seqid)
    args = checkUserAge_args()
    args.carrier = carrier
    args.sessionId = sessionId
    args.verifier = verifier
    args.standardAge = standardAge
    args.write(self._oprot)
    self._oprot.writeMessageEnd()
    self._oprot.trans.flush()

  def recv_checkUserAge(self):
    iprot = self._iprot
    (fname, mtype, rseqid) = iprot.readMessageBegin()
    if mtype == TMessageType.EXCEPTION:
      x = TApplicationException()
      x.read(iprot)
      iprot.readMessageEnd()
      raise x
    result = checkUserAge_result()
    result.read(iprot)
    iprot.readMessageEnd()
    if result.success is not None:
      return result.success
    if result.e is not None:
      raise result.e
    raise TApplicationException(TApplicationException.MISSING_RESULT, "checkUserAge failed: unknown result");

  def checkUserAgeWithDocomo(self, openIdRedirectUrl, standardAge, verifier):
    """
    Parameters:
     - openIdRedirectUrl
     - standardAge
     - verifier
    """
    self.send_checkUserAgeWithDocomo(openIdRedirectUrl, standardAge, verifier)
    return self.recv_checkUserAgeWithDocomo()

  def send_checkUserAgeWithDocomo(self, openIdRedirectUrl, standardAge, verifier):
    self._oprot.writeMessageBegin('checkUserAgeWithDocomo', TMessageType.CALL, self._seqid)
    args = checkUserAgeWithDocomo_args()
    args.openIdRedirectUrl = openIdRedirectUrl
    args.standardAge = standardAge
    args.verifier = verifier
    args.write(self._oprot)
    self._oprot.writeMessageEnd()
    self._oprot.trans.flush()

  def recv_checkUserAgeWithDocomo(self):
    iprot = self._iprot
    (fname, mtype, rseqid) = iprot.readMessageBegin()
    if mtype == TMessageType.EXCEPTION:
      x = TApplicationException()
      x.read(iprot)
      iprot.readMessageEnd()
      raise x
    result = checkUserAgeWithDocomo_result()
    result.read(iprot)
    iprot.readMessageEnd()
    if result.success is not None:
      return result.success
    if result.e is not None:
      raise result.e
    raise TApplicationException(TApplicationException.MISSING_RESULT, "checkUserAgeWithDocomo failed: unknown result");

  def retrieveOpenIdAuthUrlWithDocomo(self):
    self.send_retrieveOpenIdAuthUrlWithDocomo()
    return self.recv_retrieveOpenIdAuthUrlWithDocomo()

  def send_retrieveOpenIdAuthUrlWithDocomo(self):
    self._oprot.writeMessageBegin('retrieveOpenIdAuthUrlWithDocomo', TMessageType.CALL, self._seqid)
    args = retrieveOpenIdAuthUrlWithDocomo_args()
    args.write(self._oprot)
    self._oprot.writeMessageEnd()
    self._oprot.trans.flush()

  def recv_retrieveOpenIdAuthUrlWithDocomo(self):
    iprot = self._iprot
    (fname, mtype, rseqid) = iprot.readMessageBegin()
    if mtype == TMessageType.EXCEPTION:
      x = TApplicationException()
      x.read(iprot)
      iprot.readMessageEnd()
      raise x
    result = retrieveOpenIdAuthUrlWithDocomo_result()
    result.read(iprot)
    iprot.readMessageEnd()
    if result.success is not None:
      return result.success
    if result.e is not None:
      raise result.e
    raise TApplicationException(TApplicationException.MISSING_RESULT, "retrieveOpenIdAuthUrlWithDocomo failed: unknown result");

  def retrieveRequestToken(self, carrier):
    """
    Parameters:
     - carrier
    """
    self.send_retrieveRequestToken(carrier)
    return self.recv_retrieveRequestToken()

  def send_retrieveRequestToken(self, carrier):
    self._oprot.writeMessageBegin('retrieveRequestToken', TMessageType.CALL, self._seqid)
    args = retrieveRequestToken_args()
    args.carrier = carrier
    args.write(self._oprot)
    self._oprot.writeMessageEnd()
    self._oprot.trans.flush()

  def recv_retrieveRequestToken(self):
    iprot = self._iprot
    (fname, mtype, rseqid) = iprot.readMessageBegin()
    if mtype == TMessageType.EXCEPTION:
      x = TApplicationException()
      x.read(iprot)
      iprot.readMessageEnd()
      raise x
    result = retrieveRequestToken_result()
    result.read(iprot)
    iprot.readMessageEnd()
    if result.success is not None:
      return result.success
    if result.e is not None:
      raise result.e
    raise TApplicationException(TApplicationException.MISSING_RESULT, "retrieveRequestToken failed: unknown result");


class Processor(Iface, TProcessor):
  def __init__(self, handler):
    self._handler = handler
    self._processMap = {}
    self._processMap["checkUserAge"] = Processor.process_checkUserAge
    self._processMap["checkUserAgeWithDocomo"] = Processor.process_checkUserAgeWithDocomo
    self._processMap["retrieveOpenIdAuthUrlWithDocomo"] = Processor.process_retrieveOpenIdAuthUrlWithDocomo
    self._processMap["retrieveRequestToken"] = Processor.process_retrieveRequestToken

  def process(self, iprot, oprot):
    (name, type, seqid) = iprot.readMessageBegin()
    if name not in self._processMap:
      iprot.skip(TType.STRUCT)
      iprot.readMessageEnd()
      x = TApplicationException(TApplicationException.UNKNOWN_METHOD, 'Unknown function %s' % (name))
      oprot.writeMessageBegin(name, TMessageType.EXCEPTION, seqid)
      x.write(oprot)
      oprot.writeMessageEnd()
      oprot.trans.flush()
      return
    else:
      self._processMap[name](self, seqid, iprot, oprot)
    return True

  def process_checkUserAge(self, seqid, iprot, oprot):
    args = checkUserAge_args()
    args.read(iprot)
    iprot.readMessageEnd()
    result = checkUserAge_result()
    try:
      result.success = self._handler.checkUserAge(args.carrier, args.sessionId, args.verifier, args.standardAge)
    except TalkException, e:
      result.e = e
    oprot.writeMessageBegin("checkUserAge", TMessageType.REPLY, seqid)
    result.write(oprot)
    oprot.writeMessageEnd()
    oprot.trans.flush()

  def process_checkUserAgeWithDocomo(self, seqid, iprot, oprot):
    args = checkUserAgeWithDocomo_args()
    args.read(iprot)
    iprot.readMessageEnd()
    result = checkUserAgeWithDocomo_result()
    try:
      result.success = self._handler.checkUserAgeWithDocomo(args.openIdRedirectUrl, args.standardAge, args.verifier)
    except TalkException, e:
      result.e = e
    oprot.writeMessageBegin("checkUserAgeWithDocomo", TMessageType.REPLY, seqid)
    result.write(oprot)
    oprot.writeMessageEnd()
    oprot.trans.flush()

  def process_retrieveOpenIdAuthUrlWithDocomo(self, seqid, iprot, oprot):
    args = retrieveOpenIdAuthUrlWithDocomo_args()
    args.read(iprot)
    iprot.readMessageEnd()
    result = retrieveOpenIdAuthUrlWithDocomo_result()
    try:
      result.success = self._handler.retrieveOpenIdAuthUrlWithDocomo()
    except TalkException, e:
      result.e = e
    oprot.writeMessageBegin("retrieveOpenIdAuthUrlWithDocomo", TMessageType.REPLY, seqid)
    result.write(oprot)
    oprot.writeMessageEnd()
    oprot.trans.flush()

  def process_retrieveRequestToken(self, seqid, iprot, oprot):
    args = retrieveRequestToken_args()
    args.read(iprot)
    iprot.readMessageEnd()
    result = retrieveRequestToken_result()
    try:
      result.success = self._handler.retrieveRequestToken(args.carrier)
    except TalkException, e:
      result.e = e
    oprot.writeMessageBegin("retrieveRequestToken", TMessageType.REPLY, seqid)
    result.write(oprot)
    oprot.writeMessageEnd()
    oprot.trans.flush()


# HELPER FUNCTIONS AND STRUCTURES

class checkUserAge_args:
  """
  Attributes:
   - carrier
   - sessionId
   - verifier
   - standardAge
  """

  thrift_spec = (
    None, # 0
    None, # 1
    (2, TType.I32, 'carrier', None, None, ), # 2
    (3, TType.STRING, 'sessionId', None, None, ), # 3
    (4, TType.STRING, 'verifier', None, None, ), # 4
    (5, TType.I32, 'standardAge', None, None, ), # 5
  )

  def __init__(self, carrier=None, sessionId=None, verifier=None, standardAge=None,):
    self.carrier = carrier
    self.sessionId = sessionId
    self.verifier = verifier
    self.standardAge = standardAge

  def read(self, iprot):
    if iprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and isinstance(iprot.trans, TTransport.CReadableTransport) and self.thrift_spec is not None and fastbinary is not None:
      fastbinary.decode_binary(self, iprot.trans, (self.__class__, self.thrift_spec))
      return
    iprot.readStructBegin()
    while True:
      (fname, ftype, fid) = iprot.readFieldBegin()
      if ftype == TType.STOP:
        break
      if fid == 2:
        if ftype == TType.I32:
          self.carrier = iprot.readI32();
        else:
          iprot.skip(ftype)
      elif fid == 3:
        if ftype == TType.STRING:
          self.sessionId = iprot.readString();
        else:
          iprot.skip(ftype)
      elif fid == 4:
        if ftype == TType.STRING:
          self.verifier = iprot.readString();
        else:
          iprot.skip(ftype)
      elif fid == 5:
        if ftype == TType.I32:
          self.standardAge = iprot.readI32();
        else:
          iprot.skip(ftype)
      else:
        iprot.skip(ftype)
      iprot.readFieldEnd()
    iprot.readStructEnd()

  def write(self, oprot):
    if oprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and self.thrift_spec is not None and fastbinary is not None:
      oprot.trans.write(fastbinary.encode_binary(self, (self.__class__, self.thrift_spec)))
      return
    oprot.writeStructBegin('checkUserAge_args')
    if self.carrier is not None:
      oprot.writeFieldBegin('carrier', TType.I32, 2)
      oprot.writeI32(self.carrier)
      oprot.writeFieldEnd()
    if self.sessionId is not None:
      oprot.writeFieldBegin('sessionId', TType.STRING, 3)
      oprot.writeString(self.sessionId)
      oprot.writeFieldEnd()
    if self.verifier is not None:
      oprot.writeFieldBegin('verifier', TType.STRING, 4)
      oprot.writeString(self.verifier)
      oprot.writeFieldEnd()
    if self.standardAge is not None:
      oprot.writeFieldBegin('standardAge', TType.I32, 5)
      oprot.writeI32(self.standardAge)
      oprot.writeFieldEnd()
    oprot.writeFieldStop()
    oprot.writeStructEnd()

  def validate(self):
    return


  def __hash__(self):
    value = 17
    value = (value * 31) ^ hash(self.carrier)
    value = (value * 31) ^ hash(self.sessionId)
    value = (value * 31) ^ hash(self.verifier)
    value = (value * 31) ^ hash(self.standardAge)
    return value

  def __repr__(self):
    L = ['%s=%r' % (key, value)
      for key, value in self.__dict__.iteritems()]
    return '%s(%s)' % (self.__class__.__name__, ', '.join(L))

  def __eq__(self, other):
    return isinstance(other, self.__class__) and self.__dict__ == other.__dict__

  def __ne__(self, other):
    return not (self == other)

class checkUserAge_result:
  """
  Attributes:
   - success
   - e
  """

  thrift_spec = (
    (0, TType.I32, 'success', None, None, ), # 0
    (1, TType.STRUCT, 'e', (TalkException, TalkException.thrift_spec), None, ), # 1
  )

  def __init__(self, success=None, e=None,):
    self.success = success
    self.e = e

  def read(self, iprot):
    if iprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and isinstance(iprot.trans, TTransport.CReadableTransport) and self.thrift_spec is not None and fastbinary is not None:
      fastbinary.decode_binary(self, iprot.trans, (self.__class__, self.thrift_spec))
      return
    iprot.readStructBegin()
    while True:
      (fname, ftype, fid) = iprot.readFieldBegin()
      if ftype == TType.STOP:
        break
      if fid == 0:
        if ftype == TType.I32:
          self.success = iprot.readI32();
        else:
          iprot.skip(ftype)
      elif fid == 1:
        if ftype == TType.STRUCT:
          self.e = TalkException()
          self.e.read(iprot)
        else:
          iprot.skip(ftype)
      else:
        iprot.skip(ftype)
      iprot.readFieldEnd()
    iprot.readStructEnd()

  def write(self, oprot):
    if oprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and self.thrift_spec is not None and fastbinary is not None:
      oprot.trans.write(fastbinary.encode_binary(self, (self.__class__, self.thrift_spec)))
      return
    oprot.writeStructBegin('checkUserAge_result')
    if self.success is not None:
      oprot.writeFieldBegin('success', TType.I32, 0)
      oprot.writeI32(self.success)
      oprot.writeFieldEnd()
    if self.e is not None:
      oprot.writeFieldBegin('e', TType.STRUCT, 1)
      self.e.write(oprot)
      oprot.writeFieldEnd()
    oprot.writeFieldStop()
    oprot.writeStructEnd()

  def validate(self):
    return


  def __hash__(self):
    value = 17
    value = (value * 31) ^ hash(self.success)
    value = (value * 31) ^ hash(self.e)
    return value

  def __repr__(self):
    L = ['%s=%r' % (key, value)
      for key, value in self.__dict__.iteritems()]
    return '%s(%s)' % (self.__class__.__name__, ', '.join(L))

  def __eq__(self, other):
    return isinstance(other, self.__class__) and self.__dict__ == other.__dict__

  def __ne__(self, other):
    return not (self == other)

class checkUserAgeWithDocomo_args:
  """
  Attributes:
   - openIdRedirectUrl
   - standardAge
   - verifier
  """

  thrift_spec = (
    None, # 0
    None, # 1
    (2, TType.STRING, 'openIdRedirectUrl', None, None, ), # 2
    (3, TType.I32, 'standardAge', None, None, ), # 3
    (4, TType.STRING, 'verifier', None, None, ), # 4
  )

  def __init__(self, openIdRedirectUrl=None, standardAge=None, verifier=None,):
    self.openIdRedirectUrl = openIdRedirectUrl
    self.standardAge = standardAge
    self.verifier = verifier

  def read(self, iprot):
    if iprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and isinstance(iprot.trans, TTransport.CReadableTransport) and self.thrift_spec is not None and fastbinary is not None:
      fastbinary.decode_binary(self, iprot.trans, (self.__class__, self.thrift_spec))
      return
    iprot.readStructBegin()
    while True:
      (fname, ftype, fid) = iprot.readFieldBegin()
      if ftype == TType.STOP:
        break
      if fid == 2:
        if ftype == TType.STRING:
          self.openIdRedirectUrl = iprot.readString();
        else:
          iprot.skip(ftype)
      elif fid == 3:
        if ftype == TType.I32:
          self.standardAge = iprot.readI32();
        else:
          iprot.skip(ftype)
      elif fid == 4:
        if ftype == TType.STRING:
          self.verifier = iprot.readString();
        else:
          iprot.skip(ftype)
      else:
        iprot.skip(ftype)
      iprot.readFieldEnd()
    iprot.readStructEnd()

  def write(self, oprot):
    if oprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and self.thrift_spec is not None and fastbinary is not None:
      oprot.trans.write(fastbinary.encode_binary(self, (self.__class__, self.thrift_spec)))
      return
    oprot.writeStructBegin('checkUserAgeWithDocomo_args')
    if self.openIdRedirectUrl is not None:
      oprot.writeFieldBegin('openIdRedirectUrl', TType.STRING, 2)
      oprot.writeString(self.openIdRedirectUrl)
      oprot.writeFieldEnd()
    if self.standardAge is not None:
      oprot.writeFieldBegin('standardAge', TType.I32, 3)
      oprot.writeI32(self.standardAge)
      oprot.writeFieldEnd()
    if self.verifier is not None:
      oprot.writeFieldBegin('verifier', TType.STRING, 4)
      oprot.writeString(self.verifier)
      oprot.writeFieldEnd()
    oprot.writeFieldStop()
    oprot.writeStructEnd()

  def validate(self):
    return


  def __hash__(self):
    value = 17
    value = (value * 31) ^ hash(self.openIdRedirectUrl)
    value = (value * 31) ^ hash(self.standardAge)
    value = (value * 31) ^ hash(self.verifier)
    return value

  def __repr__(self):
    L = ['%s=%r' % (key, value)
      for key, value in self.__dict__.iteritems()]
    return '%s(%s)' % (self.__class__.__name__, ', '.join(L))

  def __eq__(self, other):
    return isinstance(other, self.__class__) and self.__dict__ == other.__dict__

  def __ne__(self, other):
    return not (self == other)

class checkUserAgeWithDocomo_result:
  """
  Attributes:
   - success
   - e
  """

  thrift_spec = (
    (0, TType.STRUCT, 'success', (AgeCheckDocomoResult, AgeCheckDocomoResult.thrift_spec), None, ), # 0
    (1, TType.STRUCT, 'e', (TalkException, TalkException.thrift_spec), None, ), # 1
  )

  def __init__(self, success=None, e=None,):
    self.success = success
    self.e = e

  def read(self, iprot):
    if iprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and isinstance(iprot.trans, TTransport.CReadableTransport) and self.thrift_spec is not None and fastbinary is not None:
      fastbinary.decode_binary(self, iprot.trans, (self.__class__, self.thrift_spec))
      return
    iprot.readStructBegin()
    while True:
      (fname, ftype, fid) = iprot.readFieldBegin()
      if ftype == TType.STOP:
        break
      if fid == 0:
        if ftype == TType.STRUCT:
          self.success = AgeCheckDocomoResult()
          self.success.read(iprot)
        else:
          iprot.skip(ftype)
      elif fid == 1:
        if ftype == TType.STRUCT:
          self.e = TalkException()
          self.e.read(iprot)
        else:
          iprot.skip(ftype)
      else:
        iprot.skip(ftype)
      iprot.readFieldEnd()
    iprot.readStructEnd()

  def write(self, oprot):
    if oprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and self.thrift_spec is not None and fastbinary is not None:
      oprot.trans.write(fastbinary.encode_binary(self, (self.__class__, self.thrift_spec)))
      return
    oprot.writeStructBegin('checkUserAgeWithDocomo_result')
    if self.success is not None:
      oprot.writeFieldBegin('success', TType.STRUCT, 0)
      self.success.write(oprot)
      oprot.writeFieldEnd()
    if self.e is not None:
      oprot.writeFieldBegin('e', TType.STRUCT, 1)
      self.e.write(oprot)
      oprot.writeFieldEnd()
    oprot.writeFieldStop()
    oprot.writeStructEnd()

  def validate(self):
    return


  def __hash__(self):
    value = 17
    value = (value * 31) ^ hash(self.success)
    value = (value * 31) ^ hash(self.e)
    return value

  def __repr__(self):
    L = ['%s=%r' % (key, value)
      for key, value in self.__dict__.iteritems()]
    return '%s(%s)' % (self.__class__.__name__, ', '.join(L))

  def __eq__(self, other):
    return isinstance(other, self.__class__) and self.__dict__ == other.__dict__

  def __ne__(self, other):
    return not (self == other)

class retrieveOpenIdAuthUrlWithDocomo_args:

  thrift_spec = (
  )

  def read(self, iprot):
    if iprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and isinstance(iprot.trans, TTransport.CReadableTransport) and self.thrift_spec is not None and fastbinary is not None:
      fastbinary.decode_binary(self, iprot.trans, (self.__class__, self.thrift_spec))
      return
    iprot.readStructBegin()
    while True:
      (fname, ftype, fid) = iprot.readFieldBegin()
      if ftype == TType.STOP:
        break
      else:
        iprot.skip(ftype)
      iprot.readFieldEnd()
    iprot.readStructEnd()

  def write(self, oprot):
    if oprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and self.thrift_spec is not None and fastbinary is not None:
      oprot.trans.write(fastbinary.encode_binary(self, (self.__class__, self.thrift_spec)))
      return
    oprot.writeStructBegin('retrieveOpenIdAuthUrlWithDocomo_args')
    oprot.writeFieldStop()
    oprot.writeStructEnd()

  def validate(self):
    return


  def __hash__(self):
    value = 17
    return value

  def __repr__(self):
    L = ['%s=%r' % (key, value)
      for key, value in self.__dict__.iteritems()]
    return '%s(%s)' % (self.__class__.__name__, ', '.join(L))

  def __eq__(self, other):
    return isinstance(other, self.__class__) and self.__dict__ == other.__dict__

  def __ne__(self, other):
    return not (self == other)

class retrieveOpenIdAuthUrlWithDocomo_result:
  """
  Attributes:
   - success
   - e
  """

  thrift_spec = (
    (0, TType.STRING, 'success', None, None, ), # 0
    (1, TType.STRUCT, 'e', (TalkException, TalkException.thrift_spec), None, ), # 1
  )

  def __init__(self, success=None, e=None,):
    self.success = success
    self.e = e

  def read(self, iprot):
    if iprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and isinstance(iprot.trans, TTransport.CReadableTransport) and self.thrift_spec is not None and fastbinary is not None:
      fastbinary.decode_binary(self, iprot.trans, (self.__class__, self.thrift_spec))
      return
    iprot.readStructBegin()
    while True:
      (fname, ftype, fid) = iprot.readFieldBegin()
      if ftype == TType.STOP:
        break
      if fid == 0:
        if ftype == TType.STRING:
          self.success = iprot.readString();
        else:
          iprot.skip(ftype)
      elif fid == 1:
        if ftype == TType.STRUCT:
          self.e = TalkException()
          self.e.read(iprot)
        else:
          iprot.skip(ftype)
      else:
        iprot.skip(ftype)
      iprot.readFieldEnd()
    iprot.readStructEnd()

  def write(self, oprot):
    if oprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and self.thrift_spec is not None and fastbinary is not None:
      oprot.trans.write(fastbinary.encode_binary(self, (self.__class__, self.thrift_spec)))
      return
    oprot.writeStructBegin('retrieveOpenIdAuthUrlWithDocomo_result')
    if self.success is not None:
      oprot.writeFieldBegin('success', TType.STRING, 0)
      oprot.writeString(self.success)
      oprot.writeFieldEnd()
    if self.e is not None:
      oprot.writeFieldBegin('e', TType.STRUCT, 1)
      self.e.write(oprot)
      oprot.writeFieldEnd()
    oprot.writeFieldStop()
    oprot.writeStructEnd()

  def validate(self):
    return


  def __hash__(self):
    value = 17
    value = (value * 31) ^ hash(self.success)
    value = (value * 31) ^ hash(self.e)
    return value

  def __repr__(self):
    L = ['%s=%r' % (key, value)
      for key, value in self.__dict__.iteritems()]
    return '%s(%s)' % (self.__class__.__name__, ', '.join(L))

  def __eq__(self, other):
    return isinstance(other, self.__class__) and self.__dict__ == other.__dict__

  def __ne__(self, other):
    return not (self == other)

class retrieveRequestToken_args:
  """
  Attributes:
   - carrier
  """

  thrift_spec = (
    None, # 0
    None, # 1
    (2, TType.I32, 'carrier', None, None, ), # 2
  )

  def __init__(self, carrier=None,):
    self.carrier = carrier

  def read(self, iprot):
    if iprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and isinstance(iprot.trans, TTransport.CReadableTransport) and self.thrift_spec is not None and fastbinary is not None:
      fastbinary.decode_binary(self, iprot.trans, (self.__class__, self.thrift_spec))
      return
    iprot.readStructBegin()
    while True:
      (fname, ftype, fid) = iprot.readFieldBegin()
      if ftype == TType.STOP:
        break
      if fid == 2:
        if ftype == TType.I32:
          self.carrier = iprot.readI32();
        else:
          iprot.skip(ftype)
      else:
        iprot.skip(ftype)
      iprot.readFieldEnd()
    iprot.readStructEnd()

  def write(self, oprot):
    if oprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and self.thrift_spec is not None and fastbinary is not None:
      oprot.trans.write(fastbinary.encode_binary(self, (self.__class__, self.thrift_spec)))
      return
    oprot.writeStructBegin('retrieveRequestToken_args')
    if self.carrier is not None:
      oprot.writeFieldBegin('carrier', TType.I32, 2)
      oprot.writeI32(self.carrier)
      oprot.writeFieldEnd()
    oprot.writeFieldStop()
    oprot.writeStructEnd()

  def validate(self):
    return


  def __hash__(self):
    value = 17
    value = (value * 31) ^ hash(self.carrier)
    return value

  def __repr__(self):
    L = ['%s=%r' % (key, value)
      for key, value in self.__dict__.iteritems()]
    return '%s(%s)' % (self.__class__.__name__, ', '.join(L))

  def __eq__(self, other):
    return isinstance(other, self.__class__) and self.__dict__ == other.__dict__

  def __ne__(self, other):
    return not (self == other)

class retrieveRequestToken_result:
  """
  Attributes:
   - success
   - e
  """

  thrift_spec = (
    (0, TType.STRUCT, 'success', (AgeCheckRequestResult, AgeCheckRequestResult.thrift_spec), None, ), # 0
    (1, TType.STRUCT, 'e', (TalkException, TalkException.thrift_spec), None, ), # 1
  )

  def __init__(self, success=None, e=None,):
    self.success = success
    self.e = e

  def read(self, iprot):
    if iprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and isinstance(iprot.trans, TTransport.CReadableTransport) and self.thrift_spec is not None and fastbinary is not None:
      fastbinary.decode_binary(self, iprot.trans, (self.__class__, self.thrift_spec))
      return
    iprot.readStructBegin()
    while True:
      (fname, ftype, fid) = iprot.readFieldBegin()
      if ftype == TType.STOP:
        break
      if fid == 0:
        if ftype == TType.STRUCT:
          self.success = AgeCheckRequestResult()
          self.success.read(iprot)
        else:
          iprot.skip(ftype)
      elif fid == 1:
        if ftype == TType.STRUCT:
          self.e = TalkException()
          self.e.read(iprot)
        else:
          iprot.skip(ftype)
      else:
        iprot.skip(ftype)
      iprot.readFieldEnd()
    iprot.readStructEnd()

  def write(self, oprot):
    if oprot.__class__ == TBinaryProtocol.TBinaryProtocolAccelerated and self.thrift_spec is not None and fastbinary is not None:
      oprot.trans.write(fastbinary.encode_binary(self, (self.__class__, self.thrift_spec)))
      return
    oprot.writeStructBegin('retrieveRequestToken_result')
    if self.success is not None:
      oprot.writeFieldBegin('success', TType.STRUCT, 0)
      self.success.write(oprot)
      oprot.writeFieldEnd()
    if self.e is not None:
      oprot.writeFieldBegin('e', TType.STRUCT, 1)
      self.e.write(oprot)
      oprot.writeFieldEnd()
    oprot.writeFieldStop()
    oprot.writeStructEnd()

  def validate(self):
    return


  def __hash__(self):
    value = 17
    value = (value * 31) ^ hash(self.success)
    value = (value * 31) ^ hash(self.e)
    return value

  def __repr__(self):
    L = ['%s=%r' % (key, value)
      for key, value in self.__dict__.iteritems()]
    return '%s(%s)' % (self.__class__.__name__, ', '.join(L))

  def __eq__(self, other):
    return isinstance(other, self.__class__) and self.__dict__ == other.__dict__

  def __ne__(self, other):
    return not (self == other)
